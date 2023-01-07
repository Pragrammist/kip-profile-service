using Prometheus;

namespace Web.Services;


public class ChildProfileMetrics
{
    public void IncCreateProfileSucceful()
    {
        ChildProfileCreatedSucc.Inc();
    }
    public void IncCreateProfileFailure()
    {
        ChildProfileCreatedFail.Inc();
    }
    private readonly Counter ChildProfileCreatedSucc = Metrics.CreateCounter("child_profile_created_successfully_total", "Number of created child profiles.");

    private readonly Counter ChildProfileCreatedFail = Metrics.CreateCounter("child_profile_created_failure_total", "Number of created child profiles failures.");
}
