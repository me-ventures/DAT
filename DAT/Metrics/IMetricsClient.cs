namespace DAT.Metrics
{
    public interface IMetricsClient
    {
        void Gauge(string metricName, double value);

        void Counter(string metricName, int value);

        void Timer(string metricName, int milliseconds);
    }
}