using System.Threading.Tasks;
using System.Net.Http;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Web;
using System;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using GrpcProfileService;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Web.GrpcInterceptors;
using Web.Services;

namespace IntegrationTests;


public class WebFixture : WebApplicationFactory<Program>, IDisposable
{
    private bool disposed = false;
    GrpcChannel grpcChannel;
    readonly string DB_NAME = "kip_profile_test_db";
    readonly string CLIENT_CONNECTION = "https://localhost:7077";


    public Profile.ProfileClient GrpcClient { get; }

    public HttpClient Client { get; }

    public WebFixture()
    {
        SetEnvironmentVariable();


        Client = CreateClient();
        var webConf = Services.GetRequiredService<IConfiguration>();
        grpcChannel = GrpcChannel.ForAddress(CLIENT_CONNECTION);
        GrpcClient = new Profile.ProfileClient(grpcChannel);
    }
    private void SetEnvironmentVariable()
    {
        Environment.SetEnvironmentVariable("DB_NAME", DB_NAME);
        Environment.SetEnvironmentVariable("ASPNETCORE_URLS", CLIENT_CONNECTION);
    }
    protected override void Dispose(bool disposing)
    {
        if (disposed) return;
        if (disposing)
        {
            var mongoDb = Services.GetRequiredService<IMongoClient>();
            mongoDb.DropDatabase(DB_NAME);
            grpcChannel.Dispose();
        }
        disposed = true;
        base.Dispose();
    }





}

[CollectionDefinition("WebContext")]
public class WebFixtireCollection : ICollectionFixture<WebFixture>, ICollectionFixture<GrpcFixture>
{

}


public class GrpcFixture : IDisposable
{
    const string CLIENT_CONNECTION = "https://localhost:7010";
    GrpcChannel grpcChannel = null!;
    WebApplication app = null!;
    WebApplicationBuilder builder = null!;
    const string TEST_DB = "profile_test_db2";
    public Profile.ProfileClient GrpcClient { get; private set; } = null!;
    Task appTask;
    public GrpcFixture()
    {
        appTask = AppBuildAndCreateGrpcChannel();
        grpcChannel = GrpcChannel.ForAddress(CLIENT_CONNECTION);

        GrpcClient = new Profile.ProfileClient(grpcChannel);
    }


    public void Dispose()
    {
        if (app is not null)
        {
            var stopTask = app.StopAsync();
            stopTask.Wait();
            var mongoClient = new MongoClient();
            mongoClient.DropDatabase(TEST_DB);
        }
        grpcChannel.Dispose();

    }
    async Task AppBuildAndCreateGrpcChannel()
    {
        Environment.SetEnvironmentVariable("DB_NAME", TEST_DB);
        Environment.SetEnvironmentVariable("ASPNETCORE_URLS", CLIENT_CONNECTION);
        builder = WebApplication.CreateBuilder(new string[] { "" });
        builder.WebHost.UseUrls(CLIENT_CONNECTION);
        builder.Services.AddGrpc(opt =>
        {
            opt.Interceptors.Add<CreateProfileMetricsInterceptor>();
        });

        builder.Services.AddSingleton<ProfileMetrics>();
        builder.Services.AddSingleton<ChildProfileMetrics>();

        Program.BuildServicesNotFromWeb(builder.Services, builder.Configuration);
        app = builder.Build();
        app.MapGrpcService<ProfileGrpcService>();
        await app.RunAsync();
    }
}