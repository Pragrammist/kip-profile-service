using Grpc.Core;
using Grpc.Core.Interceptors;
using Web.Services;

namespace Web.GrpcInterceptors;

public class CreateProfileMetricsInterceptor : Interceptor
{

    readonly ProfileMetrics _metrics;
    public CreateProfileMetricsInterceptor(ProfileMetrics metrics)
    {
        _metrics = metrics;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            var returnRes = await continuation(request, context);
            _metrics.IncCreateProfileSuccefulGrpc();
            return returnRes;
        }
        catch
        {
            _metrics.IncCreateProfileSuccefulGrpc();
            throw;
        }
    }
}