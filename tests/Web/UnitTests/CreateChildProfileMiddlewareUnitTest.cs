using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Web.Middlewares;
using Moq;
using Microsoft.AspNetCore.Http;
using System;
using Web.Services;
using Prometheus;
using System.Reflection;

namespace UnitTests;

public class CreateChildProfileMiddlewareUnitTest
{
    RequestDelegate RequestWithException => (HttpContext c) => throw new Exception("Some inner exception");

    RequestDelegate RequestWithSuccess => (HttpContext c) => Task.CompletedTask;

    RequestDelegate RequestWithStatusCode400 => (HttpContext c) => Task.CompletedTask;

    double ChildProfileCreatedSucc => GetChildCounter("ChildProfileCreatedSucc").Value;

    double ChildProfileCreatedFail => GetChildCounter("ChildProfileCreatedFail").Value;

    ChildProfileMetrics Metrics => new ChildProfileMetrics();

    CreateChildProfileMetricsMiddleware GetMiddlewareWithSuccessRequest => new CreateChildProfileMetricsMiddleware(RequestWithSuccess);

    CreateChildProfileMetricsMiddleware GetMiddlewareWithExceptionRequest => new CreateChildProfileMetricsMiddleware(RequestWithException);

    CreateChildProfileMetricsMiddleware GetMiddlewareWithFailureRequest => new CreateChildProfileMetricsMiddleware(RequestWithStatusCode400);

    HttpContext ContextWithNotCreaingChildProfile => ContextWith(200, "/somePath");

    HttpContext ContextWithCreaingChildProfile200 => ContextWith(200, "/child");

    HttpContext ContextWithCreaingChildProfile400 => ContextWith(400, "/child");

    HttpContext ContextWithCreaingChildProfile500 => ContextWith(500, "/child");



    [Fact]
    public async Task NotCreatingChildProfile()
    {
        var beforeSucc = ChildProfileCreatedSucc;
        var beforeFail = ChildProfileCreatedFail;

        await GetMiddlewareWithSuccessRequest.Invoke(ContextWithNotCreaingChildProfile, Metrics);

        ChildProfileCreatedSucc.Should().Be(beforeSucc);
        ChildProfileCreatedFail.Should().Be(beforeFail);
    }

    [Fact]
    public async Task CreatingChildProfileWithInnerException()
    {
        var beforeSucc = ChildProfileCreatedSucc;
        var beforeFail = ChildProfileCreatedFail;

        await Assert.ThrowsAsync<Exception>(() => GetMiddlewareWithExceptionRequest.Invoke(ContextWithCreaingChildProfile500, Metrics));

        ChildProfileCreatedSucc.Should().Be(beforeSucc);
        ChildProfileCreatedFail.Should().BeGreaterThan(beforeFail);
    }

    [Fact]
    public async Task CreatingChildProfileWithStatusCode400()
    {
        var beforeSucc = ChildProfileCreatedSucc;
        var beforeFail = ChildProfileCreatedFail;

        await GetMiddlewareWithSuccessRequest.Invoke(ContextWithCreaingChildProfile400, Metrics);

        ChildProfileCreatedSucc.Should().Be(beforeSucc);
        ChildProfileCreatedFail.Should().BeGreaterThan(beforeFail);
    }

    [Fact]
    public async Task CreatingChildProfileWithStatusCode200()
    {
        var beforeSucc = ChildProfileCreatedSucc;
        var beforeFail = ChildProfileCreatedFail;

        await GetMiddlewareWithSuccessRequest.Invoke(ContextWithCreaingChildProfile200, Metrics);

        ChildProfileCreatedSucc.Should().BeGreaterThan(beforeSucc);
        ChildProfileCreatedFail.Should().Be(beforeFail);
    }










    Counter GetChildCounter(string fieldName)
    {
        var typeOfMetrics = typeof(ChildProfileMetrics);

        var metricsField = typeOfMetrics.GetField(fieldName,
        BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new NullReferenceException("field doesn't found");

        var value = metricsField.GetValue(Metrics) as Counter ?? throw new NullReferenceException("value doesn't type of Metrics");

        return value;
    }

    HttpContext ContextWith(int statusCode, string path, string method = "POST")
    {
        var mock = new Mock<HttpContext>();
        mock.SetupGet(http => http.Request.Path).Returns(path);
        mock.SetupGet(http => http.Request.Method).Returns(method);
        mock.SetupGet(http => http.Response.StatusCode).Returns(statusCode);
        return mock.Object;
    }

}