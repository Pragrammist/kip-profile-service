using Prometheus;

namespace Web.Services;

public class ProfileMetrics
{
    public void IncCreateProfileSuccefulGrpc()
    {
        ChildProfileCreatedSucc.Inc();
    }
    public void IncCreateProfileFailureGrpc()
    {
        ChildProfileCreatedFail.Inc();
    }
    private readonly Counter ChildProfileCreatedSucc = Metrics.CreateCounter("grpc_profile_created_successfully_total", "Number of created child profiles.");

    private readonly Counter ChildProfileCreatedFail = Metrics.CreateCounter("grpc_profile_created_failure_total", "Number of created child profiles failures.");
}