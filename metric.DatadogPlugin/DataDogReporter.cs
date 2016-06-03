using GuaranteedRate.Metric.DatadogPlugin.Interfaces;
using GuaranteedRate.Metric.DatadogPlugin.Models;
using GuaranteedRate.Metric.DatadogPlugin.Models.Metrics;
using GuaranteedRate.Metric.DatadogPlugin.Models.Transport;
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
 * 
 */
namespace GuaranteedRate.Metric.DatadogPlugin
{
    public class DataDogReporter : ReporterBase
    {
        public const string ENVIRONMENT_TAG = "environment";
        public const string HOST_TAG = "host";

        private readonly DateTime _unixOffset = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        private readonly Metrics _metrics;
        private readonly IDictionary<string, string> _globalTags;
        private readonly double[] _histogramPercentages = { 0.75, 0.95, 0.98, 0.99, 0.999 };
        private readonly ITransport _transport;
        private readonly string[] _path;
        private readonly IMetricNameFormatter _nameFormatter;

        public DataDogReporter(Metrics metrics, ITransport transport, IMetricNameFormatter formatter, IDictionary<string, string> globalTags, string[] path)
            : base(new TextMessageWriter(), metrics)
        {
            _metrics = metrics;
            _globalTags = globalTags;
            _path = path;
            _transport = transport;
            _nameFormatter = formatter;
        }

        public DataDogReporter(Metrics metrics, ITransport transport, IMetricNameFormatter formatter, string environment, string host, string[] path)
            : base(new TextMessageWriter(), metrics)
        {
            _metrics = metrics;
            _path = path;
            _transport = transport;
            _nameFormatter = formatter;
            _globalTags = new Dictionary<string, string>();
            _globalTags.Add(ENVIRONMENT_TAG, environment);
            _globalTags.Add(HOST_TAG, host);
        }

        public override void Run()
        {
            // swallow/log exceptions here because exceptions in Tasks don't bubble up to the calling application
            try
            {
                IRequest request = this._transport.Prepare();

                long timestamp = (long)(DateTime.UtcNow.Subtract(_unixOffset).TotalSeconds);

                TransformMetrics(request, _metrics, timestamp);
            }
            catch (Exception)
            {
                // Intentionally left blank.
            }
        }

        /**
         * Broken out from the Run() method for unit testing
         */
        public IRequest TransformMetrics(IRequest request, Metrics metrics, long timestamp) 
        {
            foreach (var dictEntry in metrics.All)
            {
                if (dictEntry.Value is CounterMetric)
                {
                    LogCounter(request, dictEntry.Key, (CounterMetric)dictEntry.Value, timestamp);
                }
                else if (dictEntry.Value is HistogramMetric)
                {
                    LogHistogram(request, dictEntry.Key, (HistogramMetric)dictEntry.Value, timestamp);
                }
                else if (dictEntry.Value is MeterMetric)
                {
                    LogMeter(request, dictEntry.Key, (MeterMetric)dictEntry.Value, timestamp);
                }
                else if (dictEntry.Value is TimerMetric)
                {
                    LogTimer(request, dictEntry.Key, (TimerMetric)dictEntry.Value, timestamp);
                }
                else if (dictEntry.Value is GaugeMetric)
                {
                    LogGauge(request, dictEntry.Key, (GaugeMetric)dictEntry.Value, timestamp);
                }
            }
            return request;
        }

        private void LogTimer(IRequest request, MetricName metricName, TimerMetric metric, long timestamp)
        {
            LogGauge(request, metricName.Name + "." + TimerMetrics.FifteenMinuteRate.GetDatadogName(), metric.FifteenMinuteRate, timestamp);
            LogGauge(request, metricName.Name + "." + TimerMetrics.FiveMinuteRate.GetDatadogName(), metric.FiveMinuteRate, timestamp);
            LogGauge(request, metricName.Name + "." + TimerMetrics.OneMinuteRate.GetDatadogName(), metric.OneMinuteRate, timestamp);
            LogGauge(request, metricName.Name + "." + TimerMetrics.Max.GetDatadogName(), metric.Max, timestamp);
            LogGauge(request, metricName.Name + "." + TimerMetrics.Mean.GetDatadogName(), metric.Mean, timestamp);
            LogGauge(request, metricName.Name + "." + TimerMetrics.MeanRate.GetDatadogName(), metric.MeanRate, timestamp);
            LogGauge(request, metricName.Name + "." + TimerMetrics.Min.GetDatadogName(), metric.Min, timestamp);
            LogGauge(request, metricName.Name + "." + TimerMetrics.StdDev.GetDatadogName(), metric.StdDev, timestamp);
        }

        private void LogMeter(IRequest request, MetricName metricName, MeterMetric metric, long timestamp)
        {
            request.AddCounter(new DatadogCounter(_nameFormatter.Format(metricName.Name, _path), metric.Count, timestamp, _globalTags));
        }

        private void LogCounter(IRequest request, MetricName metricName, CounterMetric metric, long timestamp)
        {
            request.AddCounter(new DatadogCounter(_nameFormatter.Format(metricName.Name, _path), metric.Count, timestamp, _globalTags));
        }

        private void LogHistogram(IRequest request, MetricName metricName, HistogramMetric metric, long timestamp)
        {
            LogGauge(request, metricName.Name + "." + HistogramMetrics.Max.GetDatadogName(), metric.SampleMax, timestamp);
            LogGauge(request, metricName.Name + "." + HistogramMetrics.Min.GetDatadogName(), metric.SampleMin, timestamp);
            LogGauge(request, metricName.Name + "." + HistogramMetrics.Mean.GetDatadogName(), metric.SampleMean, timestamp);
            LogGauge(request, metricName.Name + "." + HistogramMetrics.StdDev.GetDatadogName(), metric.StdDev, timestamp);
            LogGauge(request, metricName.Name + "." + HistogramMetrics.Count.GetDatadogName(), metric.SampleCount, timestamp);

            double[] percentResults = metric.Percentiles(_histogramPercentages);
            LogGauge(request, metricName.Name + "." + HistogramMetrics.At75thPercentile.GetDatadogName(), percentResults[0], timestamp);
            LogGauge(request, metricName.Name + "." + HistogramMetrics.At95thPercentile.GetDatadogName(), percentResults[1], timestamp);
            LogGauge(request, metricName.Name + "." + HistogramMetrics.At98thPercentile.GetDatadogName(), percentResults[2], timestamp);
            LogGauge(request, metricName.Name + "." + HistogramMetrics.At99thPercentile.GetDatadogName(), percentResults[3], timestamp);
            LogGauge(request, metricName.Name + "." + HistogramMetrics.At999thPercentile.GetDatadogName(), percentResults[4], timestamp);

            metric.Clear();
        }

        private void LogGauge(IRequest request, MetricName metricName, GaugeMetric metric, long timestamp)
        {
            LogGauge(request, metricName.Name, System.Convert.ToInt64(metric.ValueAsString), timestamp);
        }

        private void LogGauge(IRequest request, string metricName, double value, long timestamp)
        {
            request.AddGauge(new DatadogGauge(_nameFormatter.Format(metricName, _path), value, timestamp, _globalTags));
        }
    }
}
