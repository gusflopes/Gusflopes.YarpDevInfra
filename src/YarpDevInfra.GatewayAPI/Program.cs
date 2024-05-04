using Microsoft.AspNetCore.Http.HttpResults;
using Yarp.ReverseProxy.Configuration;

var routes = new List<RouteConfig>();
var clusters = new List<ClusterConfig>();

var sampleRouteConfig = new RouteConfig()
{
    RouteId = "SampleRoute",
    ClusterId = "SampleCluster",
    Match = new RouteMatch() {Path = "{**catch-all}"}
    // Match = new RouteMatch { Path = "/red", Headers = new List<RouteHeader>()
    //     {
    //         new RouteHeader()
    //         {
    //         Name = "testing",
    //         Values = null,
    //         Mode = HeaderMatchMode.Exists,
    //         IsCaseSensitive = false
    //         }
    //     }
    // }
};

var sampleClusterConfig = new ClusterConfig()
{
    ClusterId = "SampleCluster",
    Destinations = new Dictionary<string, DestinationConfig>()
    {
        { "destination1", new DestinationConfig() { Address = "http://localhost:5000" } },
        // { "destination2", new DestinationConfig() { Address = "http://localhost:5001" } },
        // { "destination3", new DestinationConfig() { Address = "http://localhost:5002" } }
    }
};

routes.Add(sampleRouteConfig);
clusters.Add(sampleClusterConfig);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
    // .LoadFromMemory(routes, clusters);

var app = builder.Build();
app.MapReverseProxy();

app.MapPost("/admin/destinations", (AddDestinationRequest requestBody, InMemoryConfigProvider configProvider) =>
{
    if (requestBody != null)
    {
        var config = configProvider.GetConfig();
        var clusters = config.Clusters.ToList();
        // var cluster = clusters.FirstOrDefault(x => x.ClusterId == "SampleCluster");
        var cluster = clusters.FirstOrDefault();
        if (cluster != null)
        {
            var newDestinations = cluster.Destinations.ToDictionary();
            newDestinations.Add("destination2", new DestinationConfig() { Address = "http://localhost:5001"});

            var updatedCluster = new ClusterConfig()
            {
                ClusterId = cluster.ClusterId,
                Destinations = newDestinations
            };

            var index = clusters.FindIndex(x => x.ClusterId == cluster.ClusterId);
            clusters[index] = updatedCluster;

            configProvider.Update(config.Routes, clusters);
        }
    }
    else
    {
        return Results.BadRequest("Invalid request body");
    }

    return Results.Ok();

});

app.MapGet("admin/destinations", (InMemoryConfigProvider configProvider) =>
{
    var config = configProvider.GetConfig();
    var destinations = config
        .Clusters.
        FirstOrDefault()
        .Destinations
        .ToArray();

    return Results.Ok(destinations);

});

app.MapDelete("admin/destinations/{destinationKey}", (string destinationKey, InMemoryConfigProvider configProvider) =>
{

    var config = configProvider.GetConfig();
    var clusters = config.Clusters.ToList();
    var cluster = clusters.FirstOrDefault();

    if (cluster != null)
    {
        var destinations = cluster.Destinations.ToDictionary();
        if (destinations.ContainsKey(destinationKey))
        {
            destinations.Remove(destinationKey);

            var updatedCluster = new ClusterConfig()
            {
                ClusterId = cluster.ClusterId,
                Destinations = destinations
            };
            var index = clusters.FindIndex(x => x.ClusterId == cluster.ClusterId);
            clusters[index] = updatedCluster;

            configProvider.Update(config.Routes, clusters);

            return Results.Ok($"Destination with key '{destinationKey}' has been deleted.");
        }
        else
        {
            return Results.BadRequest($"Destination with key '{destinationKey}' not found.");
        }
    }
    else
    {
        return Results.NotFound("Cluster not found.");
    }
});

app.Run();

public record AddDestinationRequest(string Key, string Address);
