using System.Threading;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Core;
using Infrastructure;
using System;
using Core.CreateProfileDtos;
using Core.Exceptions;
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
    string RandomText() => Path.GetRandomFileName().Replace(".", string.Empty);
    DbFixture _fixture;
    public ProfileInteractorIntegrationTests(DbFixture fixture)
    {
        _fixture = fixture;
    }
    ProfileInteractor GetProfileInteractor()
    {
        var repo = new ProfileRepositoryImpl(_fixture.Collection);
        var hasher = new PasswordHasherImpl();
        var interactor = new ProfileInteractor(repo, hasher);
        return interactor;
    }
    ProfileFavouritesInteractor GetFavInteractor()
    {
        var repo = new ProfileRepositoryImpl(_fixture.Collection);
        var channel = GrpcChannel.ForAddress(CONTENT_SERIVCE_HTTP2_URL);
        var grpcContentClient = new FilmServiceProto.FilmServiceProtoClient(channel);
        var bridge = new ContentBridgeImpl(grpcContentClient);
        var favouritesInteractor = new ProfileFavouritesInteractor(repo, bridge);
        return favouritesInteractor;
    }

    [Fact]
    public async Task GetProfileByEmailOrLogin()
    {
        var interactor = GetProfileInteractor();
        var profileDto = new CreateProfileDto{
            Email = RandomText(),
            Login = RandomText(),
            Password = RandomText()
        };

        await interactor.Create(profileDto);
        
        var byEmail = interactor.GetProfile(profileDto.Email, profileDto.Password);
        var byLogin = interactor.GetProfile(profileDto.Login, profileDto.Password);

        byEmail.Should().NotBeNull();
        byLogin.Should().NotBeNull();
    }

    [Fact]
    public async Task ChangeEmail()
    {
        var interactor = GetProfileInteractor();
        var profileDto = new CreateProfileDto{
            Email = RandomText(),
            Login = RandomText(),
            Password = RandomText()
        };

        await interactor.Create(profileDto);

        var res = await interactor.ChangeEamil(profileDto.Email, profileDto.Password, RandomText());
        res.Should().BeTrue();
    }

    [Fact]
    public async Task ChangePassword()
    {
        var interactor = GetProfileInteractor();
        var profileDto = new CreateProfileDto{
            Email = RandomText(),
            Login = RandomText(),
            Password = RandomText()
        };

        await interactor.Create(profileDto);

        var res = await interactor.ChangePassword(profileDto.Email, profileDto.Password, RandomText());
        res.Should().BeTrue();
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
        var favInteractor = GetFavInteractor();
        var interactor = GetProfileInteractor();

        var filmWillWatchCounter = await CreateProfileAndFilmWithAdding("willWatchCount", interactor, favInteractor.AddWillWatch);

        filmWillWatchCounter.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task AddNotInteresting()
    {
        var favInteractor = GetFavInteractor();
        var interactor = GetProfileInteractor();

        var filmNotInterestingCounter = await CreateProfileAndFilmWithAdding("notInterestingCount", interactor, favInteractor.AddNotInteresting);

        filmNotInterestingCounter.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task AddWatched()
    {
       var favInteractor = GetFavInteractor();
        var interactor = GetProfileInteractor();

        var filmWatchedCounter = await CreateProfileAndFilmWithAdding("watchedCount", interactor, favInteractor.AddWatched);

        filmWatchedCounter.Should().BeGreaterThan(0);
    }
    

    [Fact]
    public async Task AddScore()
    {
        var favInteractor = GetFavInteractor();
        var interactor = GetProfileInteractor();

        var filmWillWatchCounter = await CreateProfileAndFilmWithAdding("scoreCount", interactor, favInteractor.AddScored);

        filmWillWatchCounter.Should().BeGreaterThan(0);
    }


    [Fact]
    public async Task DeleteNotInteresting()
    {
        var favInteractor = GetFavInteractor();
        var interactor = GetProfileInteractor();

        var filmNotInterestingCountCounter = await CreateProfileAndFilmWithAddingAndDeleting("notInterestingCount", interactor, favInteractor.AddNotInteresting, favInteractor.DeleteNotInteresting);

        filmNotInterestingCountCounter.Should().BeGreaterThanOrEqualTo(1);
    }

     [Fact]
    public async Task DeleteWillWatch()
    {
        var favInteractor = GetFavInteractor();
        var interactor = GetProfileInteractor();

        var filmWillWatchCounter = await CreateProfileAndFilmWithAddingAndDeleting("willWatchCount", interactor, favInteractor.AddWillWatch, favInteractor.DeleteWillWatch);

        filmWillWatchCounter.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task DeleteWatched()
    {
        var favInteractor = GetFavInteractor();
        var interactor = GetProfileInteractor();

        var filmWillWatchCounter = await CreateProfileAndFilmWithAddingAndDeleting("watchedCount", interactor, favInteractor.AddWatched, favInteractor.DeleteWatched);

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
