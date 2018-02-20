namespace DAT.Metrics.Mock
{
    public class MockMetricsClient : IMetricsClient
    {
        public void Gauge(string metricName, double value)
        {
            
        }

        public void Counter(string metricName, int value)
        {
            
        }

        public void Timer(string metricName, int milliseconds)
        {
            
        }
    }
}