using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TodoApi.HealthChecks;

public class RandomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        int responseTimeInMs = Random.Shared.Next(300);

        if (responseTimeInMs < 100)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy(
                    $"The response time is excellent ({responseTimeInMs}ms)"));
        }

        if (responseTimeInMs < 200)
        {
            return Task.FromResult(
                HealthCheckResult.Degraded(
                    $"The response time is greater than expected ({responseTimeInMs}ms)"));
        }
        
        return Task.FromResult(
            HealthCheckResult.Unhealthy(
                $"The response time is unacceptable ({responseTimeInMs}ms)"));
    }
}