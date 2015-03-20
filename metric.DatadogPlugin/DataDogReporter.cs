using metric.DatadogPlugin.Models;
using metrics;
using metrics.Core;
using metrics.Reporting;
using NUnit.Framework;
using StatsdClient;
using System.Collections.Generic;

/**
 * This code is mostly a C# translation of https://github.com/coursera/metrics-datadog
 */
namespace metric.DatadogPlugin
{
    public class DataDogReporter : ReporterBase
    {

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger("DataDogReporter");


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
                if (dictEntry.Value is CounterMetric)
                {
                    LogCounter(dictEntry.Key, (CounterMetric)dictEntry.Value, tags);
                }
                else if (dictEntry.Value is HistogramMetric)
                {
                    LogHistogram(dictEntry.Key, (HistogramMetric)dictEntry.Value, tags);
                }
                else if (dictEntry.Value is MeterMetric)
                {
                }
                else if (dictEntry.Value is TimerMetric)
                {
                }
                else if (dictEntry.Value is GaugeMetric)
                {
                    LogGauge(dictEntry.Key, (GaugeMetric)dictEntry.Value, tags);
                }
                else
                {
                    Log.InfoFormat("Unknown metric type {}, not sending", dictEntry.Value.GetType());
                }

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

        private void LogCounter(MetricName metricName, CounterMetric counterMetric, string[] tags) 
        {
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

        }

        private void LogHistogram(MetricName metricName, HistogramMetric histogramMetric, string[] tags)
        {
            
            /*
            foreach (var value in histogramMetric.Values)
            {
                DogStatsd.Histogram(_metricBaseName + metricName.Name, value, 1, tags);
            }
            */
            histogramMetric.Clear();

        }

        private void LogGauge(MetricName metricName, GaugeMetric metric, string[] tags)
        {
            DogStatsd.Gauge(_metricBaseName + metricName.Name, metric.ValueAsString, 1, tags);
        }

        private string BuildMetricBaseName(string applicationName, string domainName)
        {
            if (string.IsNullOrWhiteSpace(applicationName) || string.IsNullOrWhiteSpace(domainName))
                throw new InvalidMetricNameFormat();                

            return string.Format("{0}.{1}.", applicationName, domainName);
        }
    }

}
