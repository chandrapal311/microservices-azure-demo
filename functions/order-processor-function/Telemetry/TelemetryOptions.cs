namespace order_processor_function.Telemetry
{
    public class TelemetryOptions
    {
        public bool Enabled { get; set; } = false;
        public string InstrumentationKey { get; set; } = string.Empty;
        public string ServiceName { get; set; } = "OrderProcessor";
        public string ServiceVersion { get; set; } = "1.0.0";
    }
}
