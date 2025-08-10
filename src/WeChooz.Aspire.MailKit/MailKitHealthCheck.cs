using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WeChooz.Aspire.MailKit;
internal sealed class MailKitHealthCheck(IServiceProvider serviceProvider) : IHealthCheck
{
    internal const string HealthCheckNamePrefix = "MailKit_";
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var hasServiceKey = context.Registration.Name.StartsWith(HealthCheckNamePrefix);
            var serviceKey = hasServiceKey ? context.Registration.Name.Substring(HealthCheckNamePrefix.Length) : null;
            var factory = hasServiceKey ? serviceProvider.GetRequiredKeyedService<ISmtpClientFactory>(serviceKey) : serviceProvider.GetRequiredService<ISmtpClientFactory>();
            var client = await factory.GetSmtpClientAsync(cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}
