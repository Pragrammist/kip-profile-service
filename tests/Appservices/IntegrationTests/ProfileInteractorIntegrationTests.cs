using System.Threading;
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
using Infrastructure.ContentBridge.GrpcFilmService;
using System.Net.Http;
using Grpc.Net.Client;
using System.Net.Http.Json;
using Newtonsoft.Json.Linq;

namespace IntegrationTests;

public class ProfileInteractorIntegrationTests : IClassFixture<DbFixture>
{
    const string CONTENT_SERIVCE_HTTP1_URL = "http://localhost:5002/";
    const string CONTENT_SERIVCE_HTTP2_URL = "http://localhost:5003/";
    DbFixture _fixture;
    public ProfileInteractorIntegrationTests(DbFixture fixture)
    {
        _fixture = fixture;
    }
    ProfileInteractor GetProfileInteractor()
    {
        var repo = new ProfileRepositoryImpl(_fixture.Collection);
        var channel = GrpcChannel.ForAddress(CONTENT_SERIVCE_HTTP2_URL);
        
        var grpcContentClient = new FilmServiceProto.FilmServiceProtoClient(channel);
        var bridge = new ContentBridgeImpl(grpcContentClient);
        var interactor = new ProfileInteractor(repo, bridge);
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
    
    [Fact]
    public async Task AddWillWatch()
    {
        var interactor = GetProfileInteractor();

        var filmWillWatchCounter = await CreateProfileAndFilmWithAdding("willWatchCount", interactor, interactor.AddWillWatch);

        filmWillWatchCounter.Should().BeGreaterThan(0);
    }
    [Fact]
    public async Task AddWatched()
    {
        var interactor = GetProfileInteractor();

        var filmWillWatchCounter = await CreateProfileAndFilmWithAdding("watchedCount", interactor, interactor.AddWatched);

        filmWillWatchCounter.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task AddScore()
    {
        var interactor = GetProfileInteractor();

        var filmWillWatchCounter = await CreateProfileAndFilmWithAdding("scoreCount", interactor, interactor.AddScored);

        filmWillWatchCounter.Should().BeGreaterThan(0);
    }


    [Fact]
    public async Task DeleteWillWatch()
    {
        var interactor = GetProfileInteractor();

        var filmWillWatchCounter = await CreateProfileAndFilmWithAddingAndDeleting("willWatchCount", interactor, interactor.AddWillWatch, interactor.DeleteWillWatch);

        filmWillWatchCounter.Should().BeGreaterThanOrEqualTo(1);
    }
    [Fact]
    public async Task DeleteWatched()
    {
        var interactor = GetProfileInteractor();

        var filmWillWatchCounter = await CreateProfileAndFilmWithAddingAndDeleting("watchedCount", interactor, interactor.AddWatched, interactor.DeleteWatched);

        filmWillWatchCounter.Should().BeGreaterThanOrEqualTo(1);
    }


    async Task<uint> CreateProfileAndFilmWithAddingAndDeleting(string counterPropToAdd, ProfileInteractor interactor,
                                                        Func<string, string, CancellationToken, Task<bool>> addMethod,
                                                        Func<string, string, CancellationToken, Task<bool>> delMethod )
    {
        var profile1 = await CreateProfile(interactor);
        var profile2 = await CreateProfile(interactor);
        var film = await CreateFilm();
        await addMethod(profile1, film, default);
        await addMethod(profile2, film, default);


        await delMethod(profile2, film, default);


        var filmCounter = await GetFilmCounter(film, counterPropToAdd);
        return filmCounter;
    }

    async Task<string> CreateProfile(ProfileInteractor interactor)
    {
        var profile = Profile;
        var user = await interactor.Create(profile);
        return user.Id;
    }
    

    async Task<uint> CreateProfileAndFilmWithAdding(string counterPropToAdd, ProfileInteractor interactor,Func<string, string, CancellationToken, Task<bool>> methodToChangeCounter)
    {
        var profile = await CreateProfile(interactor);
        var film = await CreateFilm(); 

        await methodToChangeCounter(profile, film, default);
        var filmCounter = await GetFilmCounter(film, counterPropToAdd);
        return filmCounter;
    }
    async Task<uint> CreateProfileAndFilmWithAdding(string counterPropToAdd,ProfileInteractor interactor,Func<string, string, uint, CancellationToken, Task<bool>> methodToChangeCounter)
    {
        var profile = await CreateProfile(interactor);
        var film = await CreateFilm(); // чтобы работало
        var score = 3u;
        await methodToChangeCounter(profile, film, score, default);
        var filmCounter = await GetFilmCounter(film, counterPropToAdd);
        return filmCounter;
    } // for score
    async Task<uint> GetFilmCounter(string filmId, string counter)
    {
        var client = new HttpClient();
        var res =  await client.GetAsync($"{CONTENT_SERIVCE_HTTP1_URL}film/{filmId}");
        var jsonFilm = await res.Content.ReadAsStringAsync();
        var jobj = JObject.Parse(jsonFilm);
        var willWatch = uint.Parse(jobj[counter].ToString());
        client.Dispose();
        return willWatch;
    }
    async Task<string> CreateFilm()
    {
        var client = new HttpClient();
        var film = new 
        {
            AgeLimit = 18,
            Banner = "SOME BANNER",
            Name = "NAME",
            Description = "SOME DESC",
            Country = "COUNTRY",
            KindOfFilm = 0,
            ReleaseType = 0,
            Duration = "00:01:01",
            Release = "2023-02-25",
            StartScreening = "2023-02-25",
            EndScreening = "2023-02-25",
            Content = "CONTENT",
            Fees = 10,
            Images = new [] {"image"},
            Articles = new [] {"article"},
            Trailers = new [] {"trailer"},
            Tizers = new [] {"tizer"},
            Genres = new [] {"полное погружение"},
            Nominations = new [] {"оскар века"}, 
        };
        var res =  await client.PostAsJsonAsync(CONTENT_SERIVCE_HTTP1_URL+"film", film);
        var jsonFilm = await res.Content.ReadAsStringAsync();
        var jobj = JObject.Parse(jsonFilm);
        var id = jobj["id"].ToString();
        client.Dispose();
        return id;
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