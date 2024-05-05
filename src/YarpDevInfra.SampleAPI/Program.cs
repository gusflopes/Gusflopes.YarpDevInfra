using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

builder.Services.AddSingleton<IdentifierService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/sample", async (HttpContext context, IdentifierService service) =>
{

    var newRecord = await context.Request.ReadFromJsonAsync<IdentifierService.CreateSampleData>();
    if (newRecord is null)
    {
        return Results.BadRequest();
    }
    service.CreateData(newRecord);
    return Results.Created();
});


app.MapGet("/api/sample", (IdentifierService service) =>
{
    var response = new
    {
        appId = service.Identifier,
        records = service.Data
    };
    return response;
})
.WithName("Sample")
.WithOpenApi();

// Preciso habilitar CORS
app.UseCors("AllowAllOrigins");

app.Run();

public class IdentifierService
{
    public record SampleData(Guid Id, string Description);
    public record CreateSampleData(string Description);

    private readonly string _identifier;
    private List<SampleData> _data { get; set; } = new();
    public IdentifierService()
    {
        _identifier = new Random().Next().ToString();
        _data.Add(new SampleData(Guid.NewGuid(), "Primeiro Registro."));
    }
    public string Identifier { get => _identifier; }
    public IEnumerable<SampleData> Data { get => _data.ToArray(); }
    public void CreateData(CreateSampleData data)
    {
        _data.Add(new SampleData(Guid.NewGuid(), data.Description));
    }
}
