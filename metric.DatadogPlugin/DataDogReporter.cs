using metrics;
using metrics.Core;
using metrics.Reporting;
using NUnit.Framework;
using StatsdClient;
using System.Collections.Generic;

namespace metric.DatadogPlugin
{
    public class DataDogReporter : ReporterBase
    {
        private readonly Metrics _metrics;
        private readonly string _environmentTag;

        public DataDogReporter(Metrics metrics, string dataDogAgentServerName, int dataDogListeningPort, string environmentTag)
            : base(new TextMessageWriter(), metrics)
        {
            _metrics = metrics;
            _environmentTag = environmentTag;

            var dogStatsdConfig = new StatsdConfig
            {
                StatsdServerName = dataDogAgentServerName,
                StatsdPort = dataDogListeningPort,
            };

            DogStatsd.Configure(dogStatsdConfig);
        }

        public override void Run()
        {
            string[] tags = null;

            if (!string.IsNullOrEmpty(_environmentTag))
                tags = new List<string>() { string.Format("environment: {0}", _environmentTag) }.ToArray();

            foreach (var dictEntry in _metrics.All)
            {
                if (TryLogCounter(dictEntry.Key, dictEntry.Value, tags))
                    continue;

                if (TryLogHistogram(dictEntry.Key, dictEntry.Value, tags))
                    continue;

                if (dictEntry.Value is GaugeMetric)
                    TryLogGauge(dictEntry.Key, (GaugeMetric)dictEntry.Value, tags);
            }
        }

        private bool TryLogCounter(MetricName metricName, IMetric metric, string[] tags)
        {
            var counterMetric = metric as CounterMetric;
            if (counterMetric == null)
                return false;

            DogStatsd.Counter(metricName.Name, counterMetric.Count, 1, tags);

            return true;
        }

        private bool TryLogHistogram(MetricName metricName, IMetric metric, string[] tags)
        {
            var histogramMetric = metric as HistogramMetric;
            if (histogramMetric == null)
                return false;

            foreach (var value in histogramMetric.Values)
            {
                DogStatsd.Histogram(metricName.Name, value, 1, tags);
            }

            return true;
        }

        private bool TryLogGauge(MetricName metricName, GaugeMetric metric, string[] tags)
        {
            DogStatsd.Gauge(metricName.Name, metric.ValueAsString, 1, tags);

            return true;
        }
    }
}
