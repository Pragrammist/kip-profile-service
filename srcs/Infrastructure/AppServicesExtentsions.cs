using Microsoft.Extensions.DependencyInjection;
using Infrastructure.ContentBridge;
using Core;

namespace Infrastructure;

public static class AppServicesExtentsions
{
    static string GrpcContentServiceUri => Environment.GetEnvironmentVariable("CONTENT_SERVICE_GRPC_URI") ?? "http://localhost:5003";
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<ProfileRepository, ProfileRepositoryImpl>();
        services.AddSingleton<ProfileInteractor>();
        services.AddSingleton<Core.ContentBridge, ContentBridgeImpl>();
        services.AddSingleton<ProfileFavouritesInteractor>();
        services.AddTransient<PasswordHasher, PasswordHasherImpl>();
        services.AddBridgeGrpcClient(GrpcContentServiceUri);
        return services;
    }
}