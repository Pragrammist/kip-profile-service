using ProfileService.Core;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ProfileMongoExtentsions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, string connection, string dbName, string mongoCollectionName)
    {
        services.AddSingleton<IMongoClient>(p =>
        {
            var mongoClient = new MongoClient(connection);
            return mongoClient;
        });
        services.AddSingleton<IMongoDatabase>(p =>
        {
            var mongo = p.GetRequiredService<IMongoClient>();
            var db = mongo.GetDatabase(dbName);
            ProfileMongodbBuilder bdConfiguration = new ProfileMongodbBuilderImpl();
            bdConfiguration.Build();
            return db;
        });
        services.AddScoped<IMongoCollection<Profile>>(p =>
        {
            var db = p.GetRequiredService<IMongoDatabase>();
            return db.GetCollection<Profile>(mongoCollectionName);
        });
        return services;
    }


}