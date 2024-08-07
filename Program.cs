using Microsoft.Extensions.Azure;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddAzureClients(clientBuilder =>
// {
//     clientBuilder.AddClient(new CosmosClient());
//     clientBuilder.UseCredential(new DefaultAzureCredential());
// });


// New instance of CosmosClient class using a connection string
// using CosmosClient client = new(
//     connectionString: Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING")!
// );

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/test", () =>
{
    Console.WriteLine("Running read test...");
    Incident.Incident incident = new("123");

    return incident;
})
.WithName("Test")
.WithOpenApi();


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/ping", () =>
{
    return "pong";
})
.WithName("Ping")
.WithOpenApi();


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
