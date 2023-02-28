using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Web;
using System;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using GrpcProfileService;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;

namespace IntegrationTests;

public class WebFixture : WebApplicationFactory<Program>, IDisposable
{
    private bool disposed = false;
    
    readonly string DB_NAME = "kip_profile_test_db";
    readonly string CLIENT_CONNECTION_TO_GRPC_CHANEL = "http://localhost:5001";
    public Profile.ProfileClient GrpcClient { get; }

    public HttpClient HttpClient { get; }

    public WebFixture()
    {
        SetEnvironmentVariable();


        HttpClient = CreateClient();
        
        GrpcClient = Services.GetRequiredService<Profile.ProfileClient>();
    }
    private void SetEnvironmentVariable()
    {
        Environment.SetEnvironmentVariable("DB_NAME", DB_NAME);
    }
    protected override void Dispose(bool disposing)
    {
        if (disposed) return;
        if (disposing)
        {
            var mongoDb = Services.GetRequiredService<IMongoClient>();
            mongoDb.DropDatabase(DB_NAME);
            
        }
        disposed = true;
        base.Dispose();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test").ConfigureServices(services =>
        {
            services
                .AddGrpcClient<Profile.ProfileClient>(options => options.Address = new Uri(CLIENT_CONNECTION_TO_GRPC_CHANEL))
                .ConfigurePrimaryHttpMessageHandler(() => this.Server.CreateHandler());
        });
    }



}
