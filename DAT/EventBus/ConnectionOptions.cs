namespace DAT.EventBus
{
    public abstract class ConnectionOptions
    {
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public string Hostname { get; set; }
        
        public int Port { get; set; }
    }
}