using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Destructurers;
using Serilog.Exceptions.MongoDB.Destructurers;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Formatting.Json;
using TestConsoleApp;

var builder = Host.CreateApplicationBuilder();
builder.Services.AddSerilog(configureLogger =>
{
    configureLogger
        .WriteTo.Console(formatter: new JsonFormatter())
        .Enrich.WithExceptionDetails(
            new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers(new ExceptionDestructurer[]
                {
                    new MongoExceptionDestructurer()
                }));
});


var connectionString = "mongodb://root:password@localhost:27017?authSource=admin&retryWrites=true";
var client = new MongoClient(connectionString);
var database = client.GetDatabase("db01");
builder.Services.AddSingleton(database);
builder.Services.AddHostedService<AppService>();

var app = builder.Build();
await app.RunAsync();
