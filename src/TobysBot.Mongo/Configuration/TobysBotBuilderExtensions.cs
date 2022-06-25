using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using TobysBot.Configuration;
using TobysBot.Data;
using TobysBot.Mongo.Client;
using TobysBot.Mongo.Data;

namespace TobysBot.Mongo.Configuration;

public static class TobysBotBuilderExtensions
{
    /// <summary>
    /// Adds a MongoDB database to Toby's Bot with the specified <see cref="IConfiguration"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static TobysBotBuilder AddMongoDatabase(this TobysBotBuilder builder, IConfiguration configuration)
    {
        var options = configuration.Get<MongoOptions>();

        builder.Services.Configure<MongoOptions>(configuration);

        return AddDatabase(builder, options);
    }

    /// <summary>
    /// Adds a MongoDB database to Toby's Bot with the specified <see cref="MongoOptions"/> action.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static TobysBotBuilder AddMongoDatabase(this TobysBotBuilder builder, Action<MongoOptions> configureOptions)
    {
        var options = new MongoOptions();
        configureOptions(options);

        builder.Services.Configure(configureOptions);

        return AddDatabase(builder, options);
    }
    
    private static TobysBotBuilder AddDatabase(TobysBotBuilder builder, MongoOptions options)
    {
        BsonClassMap.RegisterClassMap<Entity>(cm =>
        {
            cm.AutoMap();
            cm.MapIdProperty(x => x.Id).SetIgnoreIfNull(true).SetDefaultValue(new ObjectId());
        });
        
        return builder.AddDatabase<MongoDataAccess>(services =>
        {
            services.AddSingleton<IMongoService, MongoService>();
        });
    }
}