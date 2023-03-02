using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Infrastructure.ContentBridge.GrpcFilmService;
namespace Infrastructure.ContentBridge;

public static class ContentBridgeExtensions
{
    public static IServiceCollection AddBridgeGrpcClient(this IServiceCollection services, string grpcUri)
    {
        services.AddGrpcClient<FilmServiceProto.FilmServiceProtoClient>(o =>
        {
            o.Address = new Uri(grpcUri);
        });
        
        return services;
    }
}