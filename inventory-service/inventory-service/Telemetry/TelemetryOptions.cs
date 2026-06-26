namespace inventory_service.Telemetry
{
    public class TelemetryOptions
    {
        public bool Enabled { get; set; } = false;
        public string InstrumentationKey { get; set; } = string.Empty;
        public string ServiceName { get; set; } = "InventoryService";
        public string ServiceVersion { get; set; } = "1.0.0";
    }
}
