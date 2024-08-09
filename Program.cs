using Microsoft.Extensions.Azure;
using Microsoft.Azure.Cosmos;
using IncidentRecord;
using Microsoft.OpenApi.Any;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddAzureClients(clientBuilder =>
// {
//     clientBuilder.AddClient(new CosmosClient());
//     clientBuilder.UseCredential(new DefaultAzureCredential());
// });


// New instance of CosmosClient class using a connection string
using CosmosClient client = new(
    connectionString: Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING")!
);

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

List<Incident> incidentCache = [];
DateTime lastCached = new();

app.MapGet("/test_internal_cache", async () =>
{
    Console.WriteLine("Running read test with internal cache...");
    if (DateTime.Now - lastCached >= new TimeSpan(0, 1, 0))
    {
        Database db = client.GetDatabase("test");
        Container container = db.GetContainer("test");

        using FeedIterator<Incident> feed = container.GetItemQueryIterator<Incident>(
            queryText: "SELECT * FROM incidents i"
        );

        List<Incident> incidents = [];
        List<string> queryLog = [];
        int numQueries = 0;

        // Return all paginated results
        while (feed.HasMoreResults)
        {

            FeedResponse<Incident> response = await feed.ReadNextAsync();
            queryLog.Add($"Got {response.Count} incidents from query number: {++numQueries}");

            // Iterate query results
            foreach (Incident i in response)
            {
                incidents.Add(i);
            }
        }

        lastCached = DateTime.Now;
        incidentCache = incidents;

        queryLog.Add($"Fetched {incidents.Count} incidents in {numQueries} queries from Cosmos\n");

        queryLog.Add(incidents.Count == 0 ? "no incident data" : incidents[0].ToString());

        queryLog.ForEach(Console.WriteLine);
        return string.Join('\n', queryLog);
    }
    var incidentString = incidentCache.Count == 0 ? "no incident data" : incidentCache[0].ToString();
    return $"Using cached response from: {lastCached}\n\n{incidentCache.Count} incidents retrieved from internal cache.\n\n{incidentString}";
})
.WithName("TestInternalCache")
.WithOpenApi();



app.MapGet("/test", async () =>
{
    Console.WriteLine("Running read test...");
    Database db = client.GetDatabase("test");
    Container container = db.GetContainer("test");

    using FeedIterator<Incident> feed = container.GetItemQueryIterator<Incident>(
        queryText: "SELECT * FROM incidents i"
    );

    List<Incident> incidents = [];
    List<string> queryLog = [];
    int numQueries = 0;

    // Return all paginated results
    while (feed.HasMoreResults)
    {

        FeedResponse<Incident> response = await feed.ReadNextAsync();
        queryLog.Add($"Got {response.Count} incidents from query number: {++numQueries}");

        // Iterate query results
        foreach (Incident i in response)
        {
            incidents.Add(i);
        }
    }

    queryLog.Add($"Fetched {incidents.Count} incidents in {numQueries} queries from Cosmos\n");

    queryLog.Add(incidents.Count == 0 ? "no incident data" : incidents[0].ToString());

    queryLog.ForEach(Console.WriteLine);
    return string.Join('\n', queryLog);
})
.WithName("Test")
.WithOpenApi();

app.MapGet("/ping", () =>
{
    return "pong";
})
.WithName("Ping")
.WithOpenApi();


app.Run();