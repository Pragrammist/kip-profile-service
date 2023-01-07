using System;
using Infrastructure;
using ProfileService.Core;
using MongoDB.Driver;
using Xunit;

namespace IntegrationTests;

public class MongoDbTestBase : IDisposable
{

    public IMongoClient Client { get; }
    public IMongoDatabase Db { get; }
    public const string DB_NAME = "kip-test-profile-db";
    public const string COLECTION_NAME = "profiles";
    public IMongoCollection<Profile> Repo { get; }

    public MongoDbTestBase()
    {
        Client = new MongoClient("mongodb://localhost:27017");
        Db = Client.GetDatabase(DB_NAME);
        Build();
        Repo = Db.GetCollection<Profile>(COLECTION_NAME);
    }

    public readonly InsertOneOptions _insOpt = new InsertOneOptions { };

    public readonly FilterDefinition<Profile> allFilter = Builders<Profile>.Filter.Empty;

    public void Dispose()
    {
        Client.DropDatabase(DB_NAME);
    }
    void Build()
    {
        ProfileMongodbBuilder mongoBuilder = new ProfileMongodbBuilderImpl();
        mongoBuilder.Build();
    }

}

[CollectionDefinition("MongoDb")]
public class MongoDbTestCollectionBase : ICollectionFixture<MongoDbTestBase>
{

}

