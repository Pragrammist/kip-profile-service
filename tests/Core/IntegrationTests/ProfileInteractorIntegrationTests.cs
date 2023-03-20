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
using Microsoft.Extensions.Configuration;

namespace IntegrationTests;

public class ProfileInteractorIntegrationTests : IClassFixture<DbFixture>
{
    const string CONTENT_SERIVCE_HTTP1_URL = "http://localhost:5002/";
    const string CONTENT_SERIVCE_HTTP2_URL = "http://localhost:5003/";
    string RandomText() => Path.GetRandomFileName().Replace(".", string.Empty);
    readonly DbFixture _fixture;
    readonly ProfileInteractor _interactor;
    readonly string _emailToSend;
    readonly ProfileFavouritesInteractor _favInteractor;
    public ProfileInteractorIntegrationTests(DbFixture fixture)
    {
        _emailToSend = "vmk.human4@gmail.com";
        _fixture = fixture;
        _interactor = GetProfileInteractor();
        _favInteractor = GetFavInteractor();
    }

    EmailSenderOptions EmailSenderOptions => new ()
    {
        SmtpPort = 587,
        SmtpServer = "smtp-relay.sendinblue.com",
        SmtpEmailCrend = "vitalcik.kovalenko2019@gmail.com",
        SmtpPasswordCrend = "Eb0A1PZj3sQrWq4v"
    };
    ProfileInteractor GetProfileInteractor()
    {
        var repo = new ProfileRepositoryImpl(_fixture.Collection);
        var hasher = new PasswordHasherImpl();
        var passwordCodeStore = new ResetPasswordCodeStoreImpl();
        var emailSender = new EmailSenderImpl(EmailSenderOptions);
        var interactor = new ProfileInteractor(repo, hasher, passwordCodeStore, emailSender);
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
        
        var profileDto = new CreateProfileDto{
            Email = RandomText(),
            Login = RandomText(),
            Password = RandomText()
        };

        await _interactor.Create(profileDto);
        
        var byEmail = _interactor.GetProfile(profileDto.Email, profileDto.Password);
        var byLogin = _interactor.GetProfile(profileDto.Login, profileDto.Password);

        byEmail.Should().NotBeNull();
        byLogin.Should().NotBeNull();
    }

    [Fact]
    public async Task ChangeEmail()
    {
        
        var profileDto = new CreateProfileDto{
            Email = RandomText(),
            Login = RandomText(),
            Password = RandomText()
        };

        await _interactor.Create(profileDto);

        var res = await _interactor.ChangeEamil(profileDto.Email, profileDto.Password, RandomText());
        res.Should().BeTrue();
    }

    [Fact]
    public async Task ChangePassword()
    {
        
        var profileDto = new CreateProfileDto{
            Email = RandomText(),
            Login = RandomText(),
            Password = RandomText()
        };

        await _interactor.Create(profileDto);

        var res = await _interactor.ChangePassword(profileDto.Email, profileDto.Password, RandomText());
        res.Should().BeTrue();
    }

    [Fact]
    public async Task AddUser()
    {
        var user = await _interactor.Create(Profile());

        user.Id.Should().NotBeNull();
    }

    

    [Fact]
    public async Task AddExistingUser()
    {
        
        var profile = Profile(); 
        var user = await _interactor.Create(profile);



        await Assert.ThrowsAsync<UserAlreadyExistsException>(async () =>
        {
            await _interactor.Create(profile);
        });
    }
    
    [Fact]
    public async Task AddWillWatch()
    {

        var filmWillWatchCounter = await CreateProfileAndFilmWithAdding("willWatchCount", _favInteractor.AddWillWatch);

        filmWillWatchCounter.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task AddNotInteresting()
    {

        var filmNotInterestingCounter = await CreateProfileAndFilmWithAdding("notInterestingCount", _favInteractor.AddNotInteresting);

        filmNotInterestingCounter.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task AddWatched()
    {


        var filmWatchedCounter = await CreateProfileAndFilmWithAdding("watchedCount", _favInteractor.AddWatched);

        filmWatchedCounter.Should().BeGreaterThan(0);
    }
    [Fact]
    public async Task ResetPasswordTest()
    {
        var profile = Profile();
        profile.Email = _emailToSend;
        await _interactor.Create(profile);


        var code = await _interactor.SendCodeToEmail(_emailToSend) ?? throw new NullReferenceException("code is null");
        
        var reseted = await _interactor.ResetPassword(_emailToSend, code, "newPassword");
        
        reseted.Should().BeTrue();
    }

    [Fact]
    public async Task AddScore()
    {


        var filmWillWatchCounter = await CreateProfileAndFilmWithAdding("scoreCount", _favInteractor.AddScored);

        filmWillWatchCounter.Should().BeGreaterThan(0);
    }


    [Fact]
    public async Task DeleteNotInteresting()
    {


        var filmNotInterestingCountCounter = await CreateProfileAndFilmWithAddingAndDeleting("notInterestingCount", _favInteractor.AddNotInteresting, _favInteractor.DeleteNotInteresting);

        filmNotInterestingCountCounter.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task DeleteWillWatch()
    {
        

        var filmWillWatchCounter = await CreateProfileAndFilmWithAddingAndDeleting("willWatchCount", _favInteractor.AddWillWatch, _favInteractor.DeleteWillWatch);

        filmWillWatchCounter.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task DeleteWatched()
    {
        

        var filmWillWatchCounter = await CreateProfileAndFilmWithAddingAndDeleting("watchedCount", _favInteractor.AddWatched, _favInteractor.DeleteWatched);

        filmWillWatchCounter.Should().BeGreaterThanOrEqualTo(1);
    }


    async Task<uint> CreateProfileAndFilmWithAddingAndDeleting(string counterPropToAdd,
                                                        Func<string, string, CancellationToken, Task<bool>> addMethod,
                                                        Func<string, string, CancellationToken, Task<bool>> delMethod )
    {
        var profile1 = await CreateProfile();
        var profile2 = await CreateProfile();
        var film = await CreateFilm();
        await addMethod(profile1, film, default);
        await addMethod(profile2, film, default);


        await delMethod(profile2, film, default);


        var filmCounter = await GetFilmCounter(film, counterPropToAdd);
        return filmCounter;
    }

    async Task<string> CreateProfileWithValidAndExistingEmail()
    {
        var profile = Profile();
        profile.Email = "vitalcik.kovalenko2019@gmail.com";
        var user = await _interactor.Create(profile);
        return user.Id;
    }

    async Task<string> CreateProfile()
    {
        var profile = Profile();
        var user = await _interactor.Create(profile);
        return user.Id;
    }
    

    async Task<uint> CreateProfileAndFilmWithAdding(string counterPropToAdd,Func<string, string, CancellationToken, Task<bool>> methodToChangeCounter)
    {
        var profile = await CreateProfile();
        var film = await CreateFilm(); 

        await methodToChangeCounter(profile, film, default);
        var filmCounter = await GetFilmCounter(film, counterPropToAdd);
        return filmCounter;
    }
    async Task<uint> CreateProfileAndFilmWithAdding(string counterPropToAdd,Func<string, string, uint, CancellationToken, Task<bool>> methodToChangeCounter)
    {
        var profile = await CreateProfile();
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
    CreateProfileDto Profile() => new CreateProfileDto
    {
        Email = Path.GetRandomFileName(),
        Login = Path.GetRandomFileName(),
        Password = Path.GetRandomFileName()
    };

    
}
