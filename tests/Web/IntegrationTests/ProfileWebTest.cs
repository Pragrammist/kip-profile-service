using System.IO;
using GenFu;
using Xunit;
using System.Threading.Tasks;
using FluentAssertions;
using System.Net.Http.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using GrpcProfileService;
using Mapster;
using Web.Services;
using System.Reflection;
using Prometheus;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

[Collection("WebContext")]
public class ProfileWebTest
{
    const string CONTENT_SERIVCE_HTTP1_URL = "http://localhost:5002/";
    const string PROFILE_URL = "profile";
    string RandomText => Path.GetRandomFileName().Replace(".", string.Empty);
    class UserToSend
    {
        public string Login { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }

    readonly WebFixture _webFixture;

    public ProfileWebTest(WebFixture webContext)
    {
        _webFixture = webContext;
    }
    

    [Fact]
    public async Task CreateProfileGrpc()
    {
        var profileData = User().Adapt<CreateProfileRequest>();
        var res = await _webFixture.GrpcClient.CreateProfileAsync(profileData);
        res.Id.Should().NotBeNull();
    }

    [Fact]
    public async Task LoginGrpc()
    {
        var user = User();
        await _webFixture.HttpClient.PostAsJsonAsync(PROFILE_URL, user); // создание нового юзера

        var loginByEmail = await _webFixture.GrpcClient.LoginAsync(GetLoginByEmailRequest(user));

        var loginByLogin = await _webFixture.GrpcClient.LoginAsync(GetLoginByLoginRequest(user));

        loginByLogin.Should().NotBeNull();
        loginByEmail.Should().NotBeNull();
    }

    LoginRequest GetLoginByEmailRequest(UserToSend user) => new()
    {
        LoginOrEmail = user.Email,
        Password = user.Password
    };

    LoginRequest GetLoginByLoginRequest(UserToSend user) => new()
    {
        LoginOrEmail = user.Login,
        Password = user.Password
    };
    
    [Fact]
    public async Task ChangeEmailGrpc()
    {
        var user = User();
        await _webFixture.HttpClient.PostAsJsonAsync(PROFILE_URL, user); // создание нового юзера
        var changeEmailRequest = new ChangeEmailRequest 
        {
            LoginOrEmail = user.Email,
            Password = user.Password,
            NewEmail = RandomText
        };
        var changedResult = await _webFixture.GrpcClient.ChangeEmailAsync(changeEmailRequest);
        changedResult.IsChagned.Should().BeTrue();

    }
    [Fact]
    public async Task ChangePasswordGrpc()
    {
        var user = User();
        await _webFixture.HttpClient.PostAsJsonAsync(PROFILE_URL, user); // создание нового юзера
        var changePasswordRequest = new ChangePasswordRequest 
        {
            LoginOrEmail = user.Email,
            Password = user.Password,
            NewPassword = RandomText
        };
        var changedResult = await _webFixture.GrpcClient.ChangePasswordAsync(changePasswordRequest);
        changedResult.IsChagned.Should().BeTrue();
    }
    [Fact]
    public async Task CreateProfileGrpcMetrics()
    {
        var profileData = User().Adapt<CreateProfileRequest>();
        var profileMetrics = _webFixture.Services.GetRequiredService<ProfileMetrics>();

        try
        {
            await _webFixture.GrpcClient.CreateProfileAsync(profileData);

        }
        catch { }


        var succCounter = GetChildCounter("ChildProfileCreatedSucc", profileMetrics).Value;
        var failCounter = GetChildCounter("ChildProfileCreatedFail", profileMetrics).Value;

        Assert.True(succCounter > 0 || failCounter > 0);
    }

    [Fact]
    public async Task Get()
    {
        var userToPost = User();
        var postResponseMessage = await _webFixture.HttpClient.PostAsJsonAsync(PROFILE_URL, userToPost);
        string id = await GetUserId(postResponseMessage);
        var requestParams = $"?id={id}";
        var responseMessage = await _webFixture.HttpClient.GetAsync(PROFILE_URL + requestParams);

        responseMessage.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task Post()
    {
        var userToPost = User();

        var responseMessage = await _webFixture.HttpClient.PostAsJsonAsync(PROFILE_URL, userToPost);

        responseMessage.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task LoginByEmail()
    {
        var userToPost = User();
        await _webFixture.HttpClient.PostAsJsonAsync(PROFILE_URL, userToPost); // создание нового юзера
        var loginUrl = PROFILE_URL+ "/login" + $"?loginOrEmail={userToPost.Email}&password={userToPost.Password}";

        var responseMessageByEamail = await _webFixture.HttpClient.GetAsync(loginUrl);
        
        responseMessageByEamail.EnsureSuccessStatusCode();
    }


    [Fact]
    public async Task LoginByLogin()
    {
        var userToPost = User();
        await _webFixture.HttpClient.PostAsJsonAsync(PROFILE_URL, userToPost); // создание нового юзера
        var loginUrl = PROFILE_URL+ "/login" + $"?loginOrEmail={userToPost.Login}&password={userToPost.Password}";

        var responseMessageByEamail = await _webFixture.HttpClient.GetAsync(loginUrl);
        
        responseMessageByEamail.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task AddWillWatch()
    {
        var profile = await CreateProfile();
        var film = await CreateFilm();

        var response = await _webFixture.HttpClient.PutAsJsonAsync<object>($"/willwatch/{profile}/{film}", new {});

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task DeleteWillWatch()
    {
        var profile = await CreateProfile();
        var film = await CreateFilm();
        var responseCreating = await _webFixture.HttpClient.PutAsJsonAsync<object>($"/willwatch/{profile}/{film}", new {});

        var responseDel = await _webFixture.HttpClient.PutAsJsonAsync($"/willwatch/delete/{profile}/{film}", new{});
        
        responseDel.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task AddWatched()
    {
        var profile = await CreateProfile();
        var film = await CreateFilm();

        var response = await _webFixture.HttpClient.PutAsJsonAsync<object>($"/watched/{profile}/{film}", new {});

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task DeleteWatched()
    {
        var profile = await CreateProfile();
        var film = await CreateFilm();
        var responseCreating = await _webFixture.HttpClient.PutAsJsonAsync<object>($"/watched/{profile}/{film}", new {});

        var responseDel = await _webFixture.HttpClient.PutAsJsonAsync($"/watched/delete/{profile}/{film}", new {});
        
        responseDel.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task AddNotInteresting()
    {
        var profile = await CreateProfile();
        var film = await CreateFilm();

        var response = await _webFixture.HttpClient.PutAsJsonAsync<object>($"/notinteresting/{profile}/{film}", new {});

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task DeleteNotInteresting()
    {
        var profile = await CreateProfile();
        var film = await CreateFilm();
        var responseCreating = await _webFixture.HttpClient.PutAsJsonAsync<object>($"/notinteresting/{profile}/{film}", new {});

        var responseDel = await _webFixture.HttpClient.PutAsJsonAsync($"/notinteresting/delete/{profile}/{film}", new {});
        
        responseDel.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Score()
    {
        var profile = await CreateProfile();
        var film = await CreateFilm();
        var score = 5;
        var response = await _webFixture.HttpClient.PutAsJsonAsync<object>($"/scored/{score}/{profile}/{film}", new {});

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ResetPasswordTest()
    {
        var email = "vitalcik.kovalenko2019@gmail.com";
        await CreateProfileObjectResultWithEmail(email);
        
        var sendCodeRequest = new SendCodeToResetPasswordRequest
        {
            Email = email 
        };

        var codeResponse = await _webFixture.GrpcClient.SendCodeToEmailToResetPasswordAsync(sendCodeRequest);
        
        var code = codeResponse.Code;

        var resetRequest = new ResetPasswordRequest
        {
            Code = code,
            Email = email,
            NewPassword = "mynewpassword"
        };
        var resetResponse = await _webFixture.GrpcClient.ResetPasswordAsync(resetRequest);

        
        var isReseted = resetResponse.IsReseted;

        isReseted.Should().BeTrue();
    }
    async Task<ProfileResponse> CreateProfileObjectResultWithEmail(string email)
    {
        var profileData = User().Adapt<CreateProfileRequest>();
        profileData.Email = email;
        var res = await _webFixture.GrpcClient.CreateProfileAsync(profileData);
        return res;
    }

    async Task<string> CreateProfile()
    {
        var profileData = User().Adapt<CreateProfileRequest>();
        var res = await _webFixture.GrpcClient.CreateProfileAsync(profileData);
        return res.Id;
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
    async Task<string> GetUserId(HttpResponseMessage response)
    {
        var json = await response?.Content?.ReadAsStringAsync() ?? "{}";
        var jsobj = JObject.Parse(json);
        var res = jsobj["id"].ToString();
        return res;
    }
    UserToSend User()
    {
        var user = new UserToSend
        {
            Login = RandomText,
            Email = RandomText,
            Password = RandomText
        };

        return user;
    }
    Counter GetChildCounter(string fieldName, ProfileMetrics metrics)
    {
        var typeOfMetrics = typeof(ProfileMetrics);

        var metricsField = typeOfMetrics.GetField(fieldName,
        BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new NullReferenceException("field doesn't found");

        var value = metricsField.GetValue(metrics) as Counter ?? throw new NullReferenceException("value doesn't type of Metrics");

        return value;
    }
}

