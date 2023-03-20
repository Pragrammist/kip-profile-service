using Microsoft.Extensions.DependencyInjection;
using Infrastructure.ContentBridge;
using Core;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

public static class AppServicesExtentsions
{
    static string GrpcContentServiceUri => Environment.GetEnvironmentVariable("CONTENT_SERVICE_GRPC_URI") ?? "http://localhost:5003";
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration conf)
    {
        services.AddSingleton<ProfileRepository, ProfileRepositoryImpl>();
        services.AddSingleton<ProfileInteractor>();
        services.AddSingleton<Core.ContentBridge, ContentBridgeImpl>();
        services.AddSingleton<ProfileFavouritesInteractor>();
        services.AddTransient<PasswordHasher, PasswordHasherImpl>();
        services.AddSingleton<ResetPasswordCodeStore, ResetPasswordCodeStoreImpl>();
        services.AddTransient<EmailSender, EmailSenderImpl>();
        services.AddBridgeGrpcClient(GrpcContentServiceUri);
        services.AddTransient(c => new EmailSenderOptions
        {
            SmtpEmailCrend = conf["SMTP_EMAIL_CREND"],
            SmtpPasswordCrend = conf["SMTP_PASSWORD_CREND"],
            SmtpPort = int.Parse(conf["SMTP_PORT"]),
            SmtpServer = conf["SMTP_SERVER"]
        });
        return services;
    }
}