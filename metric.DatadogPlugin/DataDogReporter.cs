using metric.DatadogPlugin.Models;
using metric.DatadogPlugin.Models.Metrics;
using metric.DatadogPlugin.Models.Transport;
using metrics;
using metrics.Core;
using metrics.Reporting;
using NUnit.Framework;
using StatsdClient;
using System;
using System.Collections.Generic;

/**
 * This code is a C# translation of https://github.com/coursera/metrics-datadog
 * built to work with the C# translation of metrics https://github.com/danielcrenna/metrics-net
 */
namespace metric.DatadogPlugin
{
    public class DataDogReporter : ReporterBase
    {

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger("DataDogReporter");
        
        private readonly DateTime unixOffset = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        private readonly Metrics _metrics;
        private readonly string _environmentTag;
        //private readonly string _metricBaseName;
        private readonly double[] histogramPercentages = { 0.75, 0.95, 0.98, 0.99, 0.999 };
        private readonly ITransport transport;
        private readonly string[] path;
        private readonly string host;
        private readonly IMetricNameFormatter formatter;

        public DataDogReporter(Metrics metrics, ITransport transport, IMetricNameFormatter formatter, string host, string[] path)
            : base(new TextMessageWriter(), metrics)
        {
            this._metrics = metrics;
            this.host = host;
            this.path = path;
            //this._environmentTag = dataDogReporterConfigModel.SourceEnvironmentTag;
            //this._metricBaseName = BuildMetricBaseName(dataDogReporterConfigModel.SourceApplicationName, dataDogReporterConfigModel.SourceDomainName);
            this.transport = transport;
            this.formatter = formatter;

            /*
            var dogStatsdConfig = new StatsdConfig
            {
                StatsdServerName = dataDogReporterConfigModel.DataDogAgentServerName,
                StatsdPort = dataDogReporterConfigModel.DataDogListeningPort,
            };

            DogStatsd.Configure(dogStatsdConfig);
             */
        }

        public override void Run()
        {
            IRequest request = this.transport.Prepare();

            string[] tags = null;

            if (!string.IsNullOrEmpty(_environmentTag))
                tags = new List<string>() { string.Format("environment: {0}", _environmentTag) }.ToArray();

            long timestamp = (long)(DateTime.UtcNow.Subtract(unixOffset).TotalSeconds);



            foreach (var dictEntry in _metrics.All)
            {
                if (dictEntry.Value is CounterMetric)
                {
                    LogCounter(request, dictEntry.Key, (CounterMetric)dictEntry.Value, timestamp, tags);
                }
                else if (dictEntry.Value is HistogramMetric)
                {
                    LogHistogram(request, dictEntry.Key, (HistogramMetric)dictEntry.Value, timestamp, tags);
                }
                else if (dictEntry.Value is MeterMetric)
                {
                }
                else if (dictEntry.Value is TimerMetric)
                {
                }
                else if (dictEntry.Value is GaugeMetric)
                {
                    LogGauge(request, dictEntry.Key, (GaugeMetric)dictEntry.Value, timestamp, tags);
                }
                else
                {
                    Log.InfoFormat("Unknown metric type {}, not sending", dictEntry.Value.GetType());
                }

            }
        }

        Dictionary<MetricName, long> _counterPrevValues = new Dictionary<MetricName, long>();

        private void LogCounter(IRequest request, MetricName metricName, CounterMetric counterMetric, long timestamp, string[] tags) 
        {
            request.AddCounter(new DatadogCounter(formatter.Format(metricName.Name, path), counterMetric.Count, timestamp, host, tags));
        }

        private void LogHistogram(IRequest request, MetricName metricName, HistogramMetric histogramMetric, long timestamp, string[] tags)
        {
            LogGauge(request, metricName.Name + ".Max", histogramMetric.SampleMax, timestamp, tags);
            LogGauge(request, metricName.Name + ".Min", histogramMetric.SampleMin, timestamp, tags);
            LogGauge(request, metricName.Name + ".Mean", histogramMetric.SampleMean, timestamp, tags);
            LogGauge(request, metricName.Name + ".StdDev", histogramMetric.StdDev, timestamp, tags);
            LogGauge(request, metricName.Name + ".Count", histogramMetric.SampleCount, timestamp, tags);

            double[] percentResults = histogramMetric.Percentiles(histogramPercentages);
            LogGauge(request, metricName.Name + ".75Percent", percentResults[0], timestamp, tags);
            LogGauge(request, metricName.Name + ".95Percent", percentResults[1], timestamp, tags);
            LogGauge(request, metricName.Name + ".98Percent", percentResults[2], timestamp, tags);
            LogGauge(request, metricName.Name + ".99Percent", percentResults[3], timestamp, tags);
            LogGauge(request, metricName.Name + ".999Percent", percentResults[4], timestamp, tags);

            histogramMetric.Clear();

        }

        private void LogGauge(IRequest request, MetricName metricName, GaugeMetric metric, long timestamp, string[] tags)
        {
            LogGauge(request, metricName.Name, System.Convert.ToInt64(metric.ValueAsString), timestamp, tags);
        }

        private void LogGauge(IRequest request, string metricName, double value, long timestamp, string[] tags)
        {
            request.AddGauge(new DatadogGauge(formatter.Format(metricName, path), value, timestamp, host, tags));
        }

        /*
        private string BuildMetricBaseName(string applicationName, string domainName)
        {
            if (string.IsNullOrWhiteSpace(applicationName) || string.IsNullOrWhiteSpace(domainName))
                throw new InvalidMetricNameFormat();                

            return string.Format("{0}.{1}.", applicationName, domainName);
        }
         */
    }

}
