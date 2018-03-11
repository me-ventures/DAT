namespace DAT.Configuration
{
    public class MetricsConfiguration
    {
        public string Type { get; set; }
        
        public string Hostname { get; set; }
        
        public int Port { get; set; }
        
        public string MetricsPrefix { get; set; }
    }
}