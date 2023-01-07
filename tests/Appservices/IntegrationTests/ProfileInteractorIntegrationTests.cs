using System.IO;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Appservices;
using Infrastructure;
using MongoDB.Driver;
using ProfileService.Core;
using System;
using Appservices.CreateProfileDtos;
using Appservices.Exceptions;

namespace IntegrationTests;

public class ProfileInteractorIntegrationTests : IClassFixture<DbFixture>
{
    DbFixture _fixture;
    public ProfileInteractorIntegrationTests(DbFixture fixture)
    {
        _fixture = fixture;
    }
    ProfileInteractor GetProfileInteractor()
    {
        var repo = new ProfileRepositoryImpl(_fixture.Collection);
        ProfileInteractor interactor = new ProfileInteractor(repo);
        return interactor;
    }

    [Fact]
    public async Task AddUser()
    {
        var interactor = GetProfileInteractor();

        var user = await interactor.Create(Profile);

        user.Id.Should().NotBeNull();
    }

    [Fact]
    public async Task AddExistingUser()
    {
        var interactor = GetProfileInteractor();
        var profile = Profile; // чтобы работало
        var user = await interactor.Create(profile);



        await Assert.ThrowsAsync<UserAlreadyExistsException>(async () =>
        {
            await interactor.Create(profile);
        });
    }

    CreateProfileDto Profile => new CreateProfileDto
    {
        Email = Path.GetRandomFileName(),
        Login = Path.GetRandomFileName(),
        Password = Path.GetRandomFileName()
    };
}

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