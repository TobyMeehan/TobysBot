using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using TobysBot.Configuration;
using TobysBot.Data;
using TobysBot.Mongo.Client;
using TobysBot.Mongo.Data;

namespace TobysBot.Mongo.Configuration;

public static class TobysBotBuilderExtensions
{
    public static TobysBotBuilder AddMongoDatabase(this TobysBotBuilder builder, IConfiguration configuration)
    {
        var options = configuration.Get<MongoOptions>();

        builder.Services.Configure<MongoOptions>(configuration);

        return AddDatabase(builder, options);
    }

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