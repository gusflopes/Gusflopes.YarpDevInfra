var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IdentifierService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/sample", (IdentifierService service) =>
{
    var response = new
    {
        Identifier = service.GetIdentifier(),
        Message = "Hello from this Sample API"
    };
    return response;
})
.WithName("Sample")
.WithOpenApi();

app.Run();

public class IdentifierService
{
    private readonly string _identifier;
    public IdentifierService()
    {
        _identifier = new Random().Next().ToString();
    }
    public string GetIdentifier()
    {
        return _identifier;
    }
}