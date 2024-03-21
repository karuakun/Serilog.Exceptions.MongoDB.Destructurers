using Microsoft.Extensions.Hosting;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace TestConsoleApp;

public class AppService(
    IMongoDatabase mongoDatabase, 
    ILogger<AppService> logger
) : IHostedService
{

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var collection = mongoDatabase.GetCollection<Restaurant>("collection");
        await collection.InsertOneAsync(new Restaurant
        {
            Name = "test"
        }, cancellationToken: cancellationToken);

        var testData = await (
                await collection.FindAsync(r => r.Name == "test", cancellationToken: cancellationToken))
            .ToListAsync(cancellationToken);

        foreach (var item in testData)
        {
            logger.LogInformation("item: {@item}", item);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}


public class Restaurant
{
    public ObjectId Id { get; set; }
    public string? Name { get; set; }
    [BsonElement("restaurant_id")]
    public string? RestaurantId { get; set; }
    public string? Cuisine { get; set; }
    public Address? Address { get; set; }
    public string? Borough { get; set; }
    public List<GradeEntry>? Grades { get; set; }
}

public class Address
{
    public string? Building { get; set; }
    [BsonElement("coord")]
    public double[]? Coordinates { get; set; }
    public string? Street { get; set; }
    [BsonElement("zipcode")]
    public string? ZipCode { get; set; }
}

public class GradeEntry
{
    public DateTime Date { get; set; }
    public string? Grade { get; set; }
    public float? Score { get; set; }
}