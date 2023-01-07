using Web.Services;

namespace Web.Middlewares;

public class CreateChildProfileMetricsMiddleware
{
    private readonly RequestDelegate _request;
    public CreateChildProfileMetricsMiddleware(RequestDelegate request)
    {
        _request = request ?? throw new ArgumentNullException(nameof(request));
    }
    public async Task Invoke(HttpContext httpContext, ChildProfileMetrics metrics)
    {
        var isCreatingChildProfile = httpContext.Request.Path == "/child" && httpContext.Request.Method == "POST";
        if (!isCreatingChildProfile)
        {
            await _request.Invoke(httpContext);
            return;
        }
        Exception? innerEcxeption = null;
        var failCounter = 0;
        try
        {
            await _request.Invoke(httpContext);
        }
        catch (Exception ex)
        {
            failCounter++;
            innerEcxeption = ex;
        }

        var statusResponseIsFailure = httpContext.Response.StatusCode == 400;

        if (statusResponseIsFailure)
            failCounter++;

        if (failCounter > 0)
            metrics.IncCreateProfileFailure();
        else
            metrics.IncCreateProfileSucceful();

        if (innerEcxeption is not null)
            throw innerEcxeption;
    }
}