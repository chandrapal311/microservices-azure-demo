using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace order_processor_function.Telemetry
{
    public static class TelemetryServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            var telemetryOptions = configuration.GetSection("Telemetry").Get<TelemetryOptions>() ?? new TelemetryOptions();

            if (!telemetryOptions.Enabled)
            {
                return services;
            }

            if (string.IsNullOrEmpty(telemetryOptions.InstrumentationKey))
            {
                throw new InvalidOperationException("Application Insights Instrumentation Key is required when telemetry is enabled.");
            }

            // Configure Resource for service identification
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(
                    serviceName: telemetryOptions.ServiceName,
                    serviceVersion: telemetryOptions.ServiceVersion)
                .AddAttributes(new Dictionary<string, object>
                {
                    { "environment", "development" },
                    { "deployment.environment", "OrderProcessor" }
                });

            services
                .AddOpenTelemetry()
                .WithTracing(traceBuilder =>
                {
                    traceBuilder
                        .SetResourceBuilder(resourceBuilder)
                        // Add HTTP client instrumentation for inter-service calls
                        .AddHttpClientInstrumentation(options =>
                        {
                            options.RecordException = true;
                        })
                        // Add Azure Monitor exporter
                        .AddAzureMonitorTraceExporter(options =>
                        {
                            options.ConnectionString = $"InstrumentationKey={telemetryOptions.InstrumentationKey}";
                        });
                });

            return services;
        }
    }
}
