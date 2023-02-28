using Microsoft.Extensions.DependencyInjection;
using Infrastructure.ContentBridge;
using Appservices;

namespace Infrastructure;

public static class AppServicesExtentsions
{
    static string GrpcContentServiceUri => Environment.GetEnvironmentVariable("CONTENT_SERVICE_GRPC_URI") ?? "http://localhost:5003";
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<ProfileRepository, ProfileRepositoryImpl>();
        services.AddSingleton<ProfileInteractor>();
        services.AddSingleton<Appservices.ContentBridge, ContentBridgeImpl>();
        services.AddBridgeClient(GrpcContentServiceUri);
        return services;
    }
}