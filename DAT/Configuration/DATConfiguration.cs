namespace DAT.Configuration
{
    public class DATConfiguration
    {
        public string Name { get; set; } = "Default-Name";
        
        public EventbusConfiguration EventBus { get; set; }
        
        public LoggingConfiguration Logging { get; set; }
    }
}