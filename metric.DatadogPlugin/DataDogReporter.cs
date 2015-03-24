using metric.DatadogPlugin.Models;
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
        private readonly string _metricBaseName;

        public DataDogReporter(Metrics metrics, DataDogReporterConfigModel dataDogReporterConfigModel)
            : base(new TextMessageWriter(), metrics)
        {
            _metrics = metrics;
            _environmentTag = dataDogReporterConfigModel.SourceEnvironmentTag;
            _metricBaseName = BuildMetricBaseName(dataDogReporterConfigModel.SourceApplicationName, dataDogReporterConfigModel.SourceDomainName);            
            
            var dogStatsdConfig = new StatsdConfig
            {
                StatsdServerName = dataDogReporterConfigModel.DataDogAgentServerName,
                StatsdPort = dataDogReporterConfigModel.DataDogListeningPort,
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

        //Dictionary<MetricName, long> _counterPrevValues = new Dictionary<MetricName, long>();

        //private bool TryLogCounter(MetricName metricName, IMetric metric)
        //{
        //    var counterMetric = metric as CounterMetric;
        //    if (counterMetric == null)
        //        return false;

        //    long valueToLog;
        //    long previousValue;

        //    if (_counterPrevValues.TryGetValue(metricName, out previousValue))
        //    {
        //        valueToLog = counterMetric.Count - previousValue;
        //        _counterPrevValues[metricName] = counterMetric.Count;
        //    }
        //    else
        //    {
        //        valueToLog = counterMetric.Count;
        //        _counterPrevValues.Add(metricName, counterMetric.Count);
        //    }

        //    DogStatsd.Counter(metricName.Name, valueToLog);

        //    return true;
        //}

        Dictionary<MetricName, long> _counterPrevValues = new Dictionary<MetricName, long>();

        private bool TryLogCounter(MetricName metricName, IMetric metric, string[] tags)
        {
            var counterMetric = metric as CounterMetric;
            if (counterMetric == null)
                return false;

            long valueToLog;
            long previousValue;

            if (_counterPrevValues.TryGetValue(metricName, out previousValue))
            {
                valueToLog = counterMetric.Count - previousValue;
                _counterPrevValues[metricName] = counterMetric.Count;
            }
            else
            {
                valueToLog = counterMetric.Count;
                _counterPrevValues.Add(metricName, counterMetric.Count);
            }

            DogStatsd.Counter(_metricBaseName + metricName.Name, valueToLog, 1, tags);

            // Set counter to zero so that we're sending the difference, not the total
            counterMetric.Clear();

            return true;
        }

        private bool TryLogHistogram(MetricName metricName, IMetric metric, string[] tags)
        {
            var histogramMetric = metric as HistogramMetric;
            if (histogramMetric == null)
                return false;

            foreach (var value in histogramMetric.Values)
            {
                DogStatsd.Histogram(_metricBaseName + metricName.Name, value, 1, tags);
            }

            histogramMetric.Clear();

            return true;
        }

        private bool TryLogGauge(MetricName metricName, GaugeMetric metric, string[] tags)
        {
            DogStatsd.Gauge(_metricBaseName + metricName.Name, metric.ValueAsString, 1, tags);

            return true;
        }

        private string BuildMetricBaseName(string applicationName, string domainName)
        {
            if (string.IsNullOrWhiteSpace(applicationName) || string.IsNullOrWhiteSpace(domainName))
                throw new InvalidMetricNameFormat();                

            return string.Format("{0}.{1}.", applicationName, domainName);
        }
    }
}
