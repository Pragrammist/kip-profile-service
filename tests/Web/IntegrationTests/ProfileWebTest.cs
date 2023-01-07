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
    class UserToSend
    {
        public string Login { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }

    readonly WebFixture _webContext;
    GrpcFixture _grpcFixture;
    public ProfileWebTest(WebFixture webContext, GrpcFixture grpcFixture)
    {
        _webContext = webContext;
        _grpcFixture = grpcFixture;
    }
    const string url = "profile";

    [Fact]
    public async Task CreateProfileGrpc()
    {
        var profileData = User().Adapt<CreateProfileRequest>();
        var res = await _grpcFixture.GrpcClient.CreateProfileAsync(profileData);
        res.Id.Should().NotBeNull();
    }


    [Fact]
    public async Task CreateProfileGrpcMetrics()
    {
        var profileData = User().Adapt<CreateProfileRequest>();
        var profileMetrics = _webContext.Services.GetRequiredService<ProfileMetrics>();

        try
        {
            await _grpcFixture.GrpcClient.CreateProfileAsync(profileData);

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
        var postResponseMessage = await _webContext.Client.PostAsJsonAsync(url, userToPost);
        string id = await GetUserId(postResponseMessage);
        var requestParams = $"?id={id}";
        var responseMessage = await _webContext.Client.GetAsync(url + requestParams);

        responseMessage.IsSuccessStatusCode.Should().BeTrue();
    }
    [Fact]
    public async Task Post()
    {
        var userToPost = User();

        var responseMessage = await _webContext.Client.PostAsJsonAsync(url, userToPost);

        responseMessage.IsSuccessStatusCode.Should().BeTrue();
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
        var user = new UserToSend { };

        user.Login = Path.GetRandomFileName();
        user.Email = Path.GetRandomFileName();
        user.Password = Path.GetRandomFileName();

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

