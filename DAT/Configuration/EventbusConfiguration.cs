namespace DAT.Configuration
{
    public class EventbusConfiguration
    {
        public string Type { get; set; }
        
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public string Hostname { get; set; }
        
        public int? Port { get; set; }
        
        public string VirtualHost { get; set; }
    }
}