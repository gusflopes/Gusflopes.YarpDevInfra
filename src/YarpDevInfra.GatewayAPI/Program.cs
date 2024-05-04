using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.HttpResults;
using Yarp.ReverseProxy;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Model;

var routes = new List<RouteConfig>();
var clusters = new List<ClusterConfig>();

var defaultRouteConfig = new RouteConfig()
{
    RouteId = "default",
    ClusterId = "default",
    Match = new RouteMatch() { Path = "{**catch-all}" }
};

var defaultCluster = new ClusterConfig()
{
    ClusterId = "default",
    Destinations = new Dictionary<string, DestinationConfig>()
    {
        { "default", new DestinationConfig() { Address = "http://localhost:5000" } }
    }
};

routes.Add(defaultRouteConfig);
clusters.Add(defaultCluster);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    // .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
    .LoadFromMemory(routes, clusters);

var app = builder.Build();
app.MapReverseProxy(proxyPipeline =>
{
    string ChooseCluster(HttpContext context)
    {
        var host = context.Request.Host.Host;
        var regex = new Regex(@"^(\d+)\.([a-zA-Z0-9]+)\.");
        var match = regex.Match(host);

        if (match.Success)
        {
            var version = match.Groups[1].Value;
            var appName = match.Groups[2].Value;
            var cluserId = $"{appName}-{version}";

            return cluserId;
        }
        return "default";
    }

    proxyPipeline.Use((context, next) =>
    {
        var lookup = context.RequestServices.GetRequiredService<IProxyStateLookup>();

        var headers = context.Request.Headers["X-Test-Version"].FirstOrDefault();
        var host = context.Request.Host;
        if (headers != null)
        {
            Console.WriteLine($"Recebido headers X-Test-Version: {headers}");
        }

        if (lookup.TryGetCluster(ChooseCluster(context), out var cluster))
        {
            context.ReassignProxyRequest(cluster);
        }
        else
        {
            Console.WriteLine($"Cluster não localizado para url: {context.Request.Host.Host}");
        }

        return next();
    });
    proxyPipeline.UseSessionAffinity();
    proxyPipeline.UseLoadBalancing();
});

app.UseRouting();

app.MapGet("/admin/clusters", (InMemoryConfigProvider configProvider) =>
{
    var config = configProvider.GetConfig();
    var clusters = config.Clusters.ToImmutableArray();

    return Results.Ok(clusters);
});

app.MapDelete("/admin/clusters/{clusterKey}", (string clusterKey, InMemoryConfigProvider configProvider) =>
{
    var config = configProvider.GetConfig();
    var clusters = config.Clusters.ToList();
    var cluster = clusters.Find(x => x.ClusterId == clusterKey);

    if (cluster == null)
        return Results.BadRequest($"Cluster with key '{clusterKey}' not found.");

    clusters.Remove(cluster);
    configProvider.Update(config.Routes, clusters);

    return Results.Ok($"Cluster with key '{clusterKey}' has been deleted.");
});

app.MapPost("/admin/clusters", (AddClusterRequest requestBody, InMemoryConfigProvider configProvider) =>
{
    if (requestBody == null)
        return Results.BadRequest("Invalid request body.");

    var config = configProvider.GetConfig();
    var clusters = config.Clusters.ToList();

    var cluster = clusters.Find(x => x.ClusterId == $"{requestBody.appName}-{requestBody.versionNumber}");

    if (cluster != null)
    {
        clusters.Remove(cluster);
    }

    var newCluster = new ClusterConfig()
    {
        ClusterId = $"{requestBody.appName}-{requestBody.versionNumber}",
        Destinations = new Dictionary<string, DestinationConfig>()
        {
            { "default", new DestinationConfig() { Address = requestBody.hostAddress } }
        }
    };

    var updatedClusters = clusters.Append(newCluster).ToList();

    configProvider.Update(config.Routes, updatedClusters);

    return Results.Ok();
});

app.Run();

public record AddClusterRequest(string appName, int versionNumber, string hostAddress);
