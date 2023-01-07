namespace Web;
using System.Reflection;
using Serilog;
using Appservices;
using Infrastructure;
using Web.Services;
using Web.Middlewares;
using Web.GrpcInterceptors;
using Prometheus;
using Serilog.Events;
using Web.YmlCustomConfiguration;
using System;
public class Program
{
    public static void BuildServicesNotFromWeb(IServiceCollection services, IConfiguration configuration)
    {
        MapsterBuilder.ConfigureMapster();
        var connection = configuration["MONGODB_CONNECTION_STRING"] ?? "mongodb://localhost:27017";
        var dbName = configuration["DB_NAME"] ?? "kip_profile_db";
        var collections = configuration["COLLECTION_NAME"] ?? "profiles";
        services.AddMongoDb(connection, dbName, collections);
        services.AddScoped<ProfileRepository, ProfileRepositoryImpl>();
        services.AddScoped<ProfileInteractor>();
    }

    public static void Main(string[] args)
    {
        try
        {
            AppBuild(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.InnerException?.Message);
            Console.WriteLine("CONFIG is " + Environment.GetEnvironmentVariable("CONFIG"));
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void AppBuild(string[] args)
    {
        MapsterBuilder.ConfigureMapster();
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddValueFromYmlVar();

        

        var logstashUrl = builder.Configuration["LOGSTASH_URL"] ?? "http://localhost:8080";
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .WriteTo.Console()
            .WriteTo.Http(logstashUrl, queueLimitBytes: null)
            .CreateLogger();

        builder.Host.UseSerilog();

        builder.Services.AddControllers();
        builder.Services.AddGrpc(opt =>
        {
            opt.Interceptors.Add<CreateProfileMetricsInterceptor>();
        });

        builder.Services.AddSingleton<ProfileMetrics>();
        builder.Services.AddSingleton<ChildProfileMetrics>();


        BuildServicesNotFromWeb(builder.Services, builder.Configuration);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), includeControllerXmlComments: true);
        });

        var app = builder.Build();
        app.UseSerilogRequestLogging();

        // if (app.Environment.IsDevelopment())
        // {

        // }
        app.UseRouting();
        app.UseMetricServer();
        app.UseHttpMetrics(options => options.ReduceStatusCodeCardinality());
        app.UseGrpcMetrics();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });
        app.UseMiddleware<CreateChildProfileMetricsMiddleware>();
        app.MapMetrics();
        app.MapControllers();
        app.MapGrpcService<ProfileGrpcService>();
        app.Run();
    }
}