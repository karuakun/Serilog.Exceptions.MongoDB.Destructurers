using Serilog.Exceptions.Destructurers;
using MongoDB.Driver;
using Serilog.Exceptions.Core;
using MongoDB.Driver.Core.Operations;

namespace Serilog.Exceptions.MongoDB.Destructurers;

/*

https://mongodb.github.io/mongo-csharp-driver/2.6/apidocs/html/T_MongoDB_Driver_MongoException.htm
   
System.Object
   System.Exception
     MongoDB.Driver.MongoException
       MongoDB.Driver.MongoClientException
         MongoDB.Driver.MongoConfigurationException
         MongoDB.Driver.MongoIncompatibleDriverException
         MongoDB.Driver.MongoWaitQueueFullException

System.Object
   System.Exception
     MongoDB.Driver.MongoException
       MongoDB.Driver.MongoConnectionException
         MongoDB.Driver.MongoAuthenticationException
         MongoDB.Driver.MongoConnectionClosedException

System.Object
   System.Exception
     MongoDB.Driver.MongoException
       MongoDB.Driver.MongoInternalException

System.ObjectMongoNodeIsRecoveringException
   System.Exception
     MongoDB.Driver.MongoException
       MongoDB.Driver.MongoServerException
         MongoDB.Driver.Core.Operations.MongoBulkWriteOperationException
         MongoDB.Driver.GridFS.MongoGridFSException
         MongoDB.Driver.MongoBulkWriteException
         MongoDB.Driver.MongoCommandException
           MongoDB.Driver.MongoWriteConcernException
           MongoDB.Driver.MongoDuplicateKeyException
         MongoDB.Driver.MongoExecutionTimeoutException
         MongoDB.Driver.MongoNodeIsRecoveringException
         MongoDB.Driver.MongoNotPrimaryException
         MongoDB.Driver.MongoQueryException
           MongoDB.Driver.MongoCursorNotFoundException
         MongoDB.Driver.MongoWriteException
 */


/// <summary>
/// 
/// </summary>
public class MongoExceptionDestructurer : ExceptionDestructurer
{
    private readonly bool _destructureCommonExceptionProperties;
    public override Type[] TargetTypes => [
        // MongoConnectionException
        typeof(MongoConnectionException),
        typeof(MongoAuthenticationException),
        typeof(MongoConnectionClosedException),

        // MongoQueryException
        typeof(MongoQueryException),
        typeof(MongoCursorNotFoundException),

        // MongoCommandException
        typeof(MongoCommandException),
        typeof(MongoWriteConcernException),
        typeof(MongoDuplicateKeyException),

        // MongoExecutionTimeoutException
        typeof(MongoExecutionTimeoutException),

        // MongoServerException
        typeof(MongoServerException),
        typeof(MongoBulkWriteOperationException),
        typeof(MongoNodeIsRecoveringException),
        typeof(MongoNotPrimaryException),
        typeof(MongoWriteException),
        typeof(MongoBulkWriteException),
        typeof(MongoBulkWriteException<>),

        // MongoException
        typeof(MongoException),
        typeof(MongoConfigurationException),
        typeof(MongoClientException),
        typeof(MongoIncompatibleDriverException),
        typeof(MongoWaitQueueFullException),
        typeof(MongoInternalException),
    ];

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoExceptionDestructurer"/> class.
    /// </summary>
    /// <param name="destructureCommonExceptionProperties">Destructure common public Exception properties or not.</param>
    public MongoExceptionDestructurer(bool destructureCommonExceptionProperties = true)
    {
        _destructureCommonExceptionProperties = destructureCommonExceptionProperties;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="propertiesBag"></param>
    /// <param name="destructureException"></param>
    public override void Destructure(
        Exception exception, 
        IExceptionPropertiesBag propertiesBag, 
        Func<Exception, IReadOnlyDictionary<string, object?>?> destructureException)
    {
        if (_destructureCommonExceptionProperties)
        {
            base.Destructure(exception, propertiesBag, destructureException);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(exception);
            ArgumentNullException.ThrowIfNull(propertiesBag);
            ArgumentNullException.ThrowIfNull(destructureException);
        }

        if (exception is not MongoException mongoException)
            return;

        propertiesBag.AddProperty(nameof(MongoException.ErrorLabels), mongoException.ErrorLabels);

        switch (exception)
        {
            case MongoConnectionException ex:
                // MongoConnectionException
                // MongoAuthenticationException
                // MongoConnectionClosedException
                propertiesBag.AddProperty(nameof(MongoConnectionException.ConnectionId), ex.ConnectionId);
                propertiesBag.AddProperty(nameof(MongoConnectionException.IsNetworkException), ex.IsNetworkException);
                propertiesBag.AddProperty(nameof(MongoConnectionException.ErrorLabels), ex.ErrorLabels);
                break;
            case MongoQueryException ex:
                // MongoQueryException
                // MongoCursorNotFoundException
                propertiesBag.AddProperty(nameof(MongoQueryException.ConnectionId), ex.ConnectionId);
                propertiesBag.AddProperty(nameof(MongoQueryException.Query), ex.Query?.ToString());
                propertiesBag.AddProperty(nameof(MongoQueryException.QueryResult), ex.QueryResult?.ToString());
                if (ex is MongoCursorNotFoundException ex2)
                {
                    propertiesBag.AddProperty(nameof(MongoCursorNotFoundException.CursorId), ex2.CursorId);
                }
                break;
            case MongoCommandException ex:
                // MongoWriteConcernException
                // MongoDuplicateKeyException
                propertiesBag.AddProperty(nameof(MongoCommandException.ConnectionId), ex.ConnectionId);
                propertiesBag.AddProperty(nameof(MongoCommandException.Command), ex.Command?.ToString());
                propertiesBag.AddProperty(nameof(MongoCommandException.Code), ex.Code);
                propertiesBag.AddProperty(nameof(MongoCommandException.CodeName), ex.CodeName);
                propertiesBag.AddProperty(nameof(MongoCommandException.ErrorMessage), ex.ErrorMessage);
                propertiesBag.AddProperty(nameof(MongoCommandException.Result), ex.Result.ToString());
                break;
            case MongoExecutionTimeoutException ex:
                propertiesBag.AddProperty(nameof(MongoExecutionTimeoutException.ConnectionId), ex.ConnectionId);
                propertiesBag.AddProperty(nameof(MongoExecutionTimeoutException.Code), ex.Code);
                propertiesBag.AddProperty(nameof(MongoExecutionTimeoutException.CodeName), ex.CodeName);

                break;
            case MongoServerException ex:
                propertiesBag.AddProperty(nameof(MongoServerException.ConnectionId), ex.ConnectionId);
                break;
            default:
                break;
        }
    }
}
