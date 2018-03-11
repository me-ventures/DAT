using DAT.Configuration;
using StatsdClient;

namespace DAT.Metrics.StatsD
{
    public class StatsDMetricsClient : IMetricsClient
    {
        public StatsDMetricsClient(MetricsConfiguration configuration)
        {
           
            StatsdClient.Metrics.Configure(new MetricsConfig
            {
                StatsdServerName = configuration.Hostname,
                StatsdServerPort = configuration.Port,
                Prefix = configuration.MetricsPrefix
            });
        }
        
        public void Gauge(string metricName, double value)
        {
            StatsdClient.Metrics.GaugeAbsoluteValue(metricName, value);
        }

        public void Counter(string metricName, int value)
        {
            StatsdClient.Metrics.Counter(metricName, value);
        }

        public void Timer(string metricName, int milliseconds)
        {
            StatsdClient.Metrics.Timer(metricName, milliseconds);
        }
    }
}