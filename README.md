## Serilog.Exceptions.MongoDB.Destructurers

Configure a Destructure for MongoDB when MongoDB is used with Serilog Exception to address the problem of huge exception messages.

## Nuget

```
dotnet add package Serilog.Exceptions.MongoDB.Destructurers
```

## Usage
```cs
    configureLogger
        .WriteTo.Console(formatter: new JsonFormatter())
        .Enrich.WithExceptionDetails(
            new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers(new ExceptionDestructurer[]
                {
                    new MongoExceptionDestructurer()
                }));
```
