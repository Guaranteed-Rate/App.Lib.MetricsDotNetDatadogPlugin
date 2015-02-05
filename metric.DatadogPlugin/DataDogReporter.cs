using metrics;
using metrics.Core;
using metrics.Reporting;
using NUnit.Framework;
using StatsdClient;

namespace metric.DatadogPlugin
{
    public class DataDogReporter : ReporterBase
    {
        private readonly Metrics _metrics;

        public DataDogReporter(Metrics metrics, string datadogAgentSvrName, int datadogListeningPort)
            : base(new TextMessageWriter(), metrics)
        {
            _metrics = metrics;

            var dogstatsdConfig = new StatsdConfig
            {
                StatsdServerName = datadogAgentSvrName,
                StatsdPort = datadogListeningPort,
            };

            DogStatsd.Configure(dogstatsdConfig);
        }

        public override void Run()
        {
            foreach (var dictEntry in _metrics.All)
            {
                if (TryLogCounter(dictEntry.Key, dictEntry.Value))
                    continue;

                if (TryLogHistogram(dictEntry.Key, dictEntry.Value))
                    continue;

                if (dictEntry.Value is GaugeMetric)
                    TryLogGauge(dictEntry.Key, (GaugeMetric)dictEntry.Value);
            }
        }

        private bool TryLogHistogram(MetricName metricName, IMetric metric)
        {
            var histogramMetric = metric as HistogramMetric;
            if (histogramMetric == null)
                return false;

            foreach (var value in histogramMetric.Values)
            {
                DogStatsd.Histogram(metricName.Name, value);
            }

            return true;
        }

        private bool TryLogCounter(MetricName metricName, IMetric metric)
        {
            var counterMetric = metric as CounterMetric;
            if (counterMetric == null)
                return false;

            DogStatsd.Counter(metricName.Name, counterMetric.Count);

            return true;
        }

        private bool TryLogGauge(MetricName metricName, GaugeMetric metric)
        {
            DogStatsd.Gauge(metricName.Name, metric.ValueAsString);

            return true;
        }
    }
}
