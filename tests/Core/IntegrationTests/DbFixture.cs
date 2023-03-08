using Infrastructure;
using MongoDB.Driver;
using ProfileService.Core;
using System;

namespace IntegrationTests;

public class DbFixture : IDisposable
{
    public IMongoCollection<Profile> Collection { get; }
    IMongoClient _client;
    const string DB_NAME = "test-profile-db";
    const string COLLECTION_NAME = "profiles";
    public DbFixture()
    {
        MapsterBuilder.ConfigureMapster();

        _client = new MongoClient("mongodb://localhost:27017");
        var db = _client.GetDatabase(DB_NAME);
        ProfileMongodbBuilder b = new ProfileMongodbBuilderImpl();
        b.Build();
        Collection = db.GetCollection<Profile>(COLLECTION_NAME);
    }
    public void Dispose()
    {
        _client.DropDatabase(DB_NAME);
    }
}