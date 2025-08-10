using MailKit;
using MailKit.Net.Smtp;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WeChooz.Aspire.MailKit;

/// <summary>
/// Provides extension methods for registering a <see cref="SmtpClient"/> as a
/// scoped-lifetime service in the services provided by the <see cref="IHostApplicationBuilder"/>.
/// </summary>
public static class MailKitExtensions
{
    internal const string HealthCheckNamePrefix = "MailKit_";
    /// <summary>
    /// Registers 'Scoped' <see cref="MailKitClientFactory" /> for creating
    /// connected <see cref="SmtpClient"/> instance for sending emails.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IServiceCollection" /> to add services to.
    /// </param>
    /// <param name="connectionString">
    /// The connection string for the SMTP server.
    /// </param>
    /// <param name="configureSettings">
    /// An optional delegate that can be used for customizing options.
    /// It's invoked after the settings are read from the configuration.
    /// </param>
    public static void AddMailKit(this IHostApplicationBuilder builder, string connectionString, Action<MailKitClientSettings>? configureSettings = null) =>
        builder.AddMailKitClient(connectionString, configureSettings, serviceKey: null);
    /// <summary>
    /// Registers 'Scoped' <see cref="MailKitClientFactory" /> for creating
    /// connected <see cref="SmtpClient"/> instance for sending emails.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IServiceCollection" /> to add services to.
    /// </param>
    /// <param name="name">The name to create the keyed instance for MailKit.</param>
    /// <param name="connectionString">
    /// The connection string for the SMTP server.
    /// </param>
    /// <param name="configureSettings">
    /// An optional delegate that can be used for customizing options.
    /// It's invoked after the settings are read from the configuration.
    /// </param>
    public static void AddMailKit(this IHostApplicationBuilder builder, string name, string connectionString, Action<MailKitClientSettings>? configureSettings = null) =>
        builder.AddMailKitClient(connectionString, configureSettings, serviceKey: name);

    private static void AddMailKitClient(this IHostApplicationBuilder builder, string connectionString, Action<MailKitClientSettings>? configureSettings, string? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var settings = new MailKitClientSettings();
        settings.ParseConnectionString(connectionString);

        configureSettings?.Invoke(settings);

        if (serviceKey is null)
        {
            builder.Services.AddScoped(CreateMailKitClientFactory);
        }
        else
        {
            builder.Services.AddKeyedScoped(serviceKey, (sp, key) => CreateMailKitClientFactory(sp));
        }

        ISmtpClientFactory CreateMailKitClientFactory(IServiceProvider _)
        {
            return new MailKitClientFactory(settings);
        }

        if (settings.DisableHealthChecks is false)
        {
            builder.Services.AddHealthChecks()
                .AddCheck<MailKitHealthCheck>(
                    name: serviceKey is null ? "MailKit" : HealthCheckNamePrefix + serviceKey,
                    failureStatus: default,
                    tags: []);
        }

        if (settings.DisableTracing is false)
        {
            builder.Services.AddOpenTelemetry()
                .WithTracing(traceBuilder => traceBuilder.AddSource(Telemetry.SmtpClient.ActivitySourceName));
        }

        if (settings.DisableMetrics is false)
        {
            // Required by MailKit to enable metrics
            Telemetry.SmtpClient.Configure();

            builder.Services.AddOpenTelemetry()
                .WithMetrics(metricsBuilder => metricsBuilder.AddMeter(Telemetry.SmtpClient.MeterName));
        }
    }
}
