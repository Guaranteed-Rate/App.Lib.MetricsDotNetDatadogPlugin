using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using metrics.Core;
using GuaranteedRate.Metric.DatadogPlugin.Interfaces;
using GuaranteedRate.Metric.DatadogPlugin.Formatters;
using metrics;
using GuaranteedRate.Metric.DatadogPlugin.Models.Transport;
using GuaranteedRate.Metric.DatadogPlugin.Tests.Models.Transport;
using GuaranteedRate.Metric.DatadogPlugin.Models.Metrics;

namespace GuaranteedRate.Metric.DatadogPlugin.Tests
{
    [TestFixture]
    public class DataDogReporterTest
    {
        public const string HOST = "TestHost";
        public const string ENVIRONMENT = "TestEnv";
        public const string APP = "TestApp";
        public const string DOMAIN = "TestDomain";

        [Test]
        public void MetricConverterTest()
        {
            var metrics = new Metrics();
            CounterMetric counter = metrics.Counter("test", "CounterMetric");
            HistogramMetric histogramMetric = metrics.Histogram("test", "HistogramMetric");
            GaugeMetric gaugeMetric = metrics.Gauge("test", "GaugeMetric", GetNumberOfUsersLoggedIn);

            DataDogReporter reporter = CreateDefaultReporter(metrics);

            TestRequest request = new TestRequest();
            reporter.TransformMetrics(request, metrics, 0);

            IDictionary<string, DatadogSeries> convertedMetrics = request.Metrics;

            Assert.AreEqual(12, convertedMetrics.Count);
        }

        [Test]
        public void SingleCounterTest()
        {
            var metrics = new Metrics();
            string metricName = "CounterMetric";
            CounterMetric counter = metrics.Counter("test", metricName);

            DataDogReporter reporter = CreateDefaultReporter(metrics);

            TestRequest request = new TestRequest();
            reporter.TransformMetrics(request, metrics, 0);

            IDictionary<string, DatadogSeries> convertedMetrics = request.Metrics;

            Assert.AreEqual(1, convertedMetrics.Count);
            DatadogSeries metric = convertedMetrics[APP + "." + DOMAIN + "." + metricName];
            Assert.NotNull(metric);
            Assert.IsTrue(metric is DatadogCounter);
            ValidateDefaultTags(metric.Tags);
        }

        [Test]
        public void SingleHistogramTest()
        {
            var metrics = new Metrics();
            string metricName = "HistogramrMetric";
            HistogramMetric histogramMetric = metrics.Histogram("test", metricName);

            DataDogReporter reporter = CreateDefaultReporter(metrics);

            TestRequest request = new TestRequest();
            reporter.TransformMetrics(request, metrics, 0);

            IDictionary<string, DatadogSeries> convertedMetrics = request.Metrics;

            Assert.AreEqual(10, convertedMetrics.Count);
            foreach (HistogramMetrics histoName in HistogramMetrics.AllMetrics)
            {
                DatadogSeries metric = convertedMetrics[APP + "." + DOMAIN + "." + metricName + "." + histoName.GetDatadogName()];
                Assert.NotNull(metric);
                Assert.IsTrue(metric is DatadogGauge);
                ValidateDefaultTags(metric.Tags);
            }
        }

        [Test]
        public void SingleTimerTest()
        {
            var metrics = new Metrics();
            string metricName = "TimerMetric";
            TimerMetric timerMetric = metrics.Timer("test", metricName, TimeUnit.Seconds, TimeUnit.Seconds);

            DataDogReporter reporter = CreateDefaultReporter(metrics);

            TestRequest request = new TestRequest();
            reporter.TransformMetrics(request, metrics, 0);

            IDictionary<string, DatadogSeries> convertedMetrics = request.Metrics;

            Assert.AreEqual(8, convertedMetrics.Count);
            foreach (TimerMetrics timerName in TimerMetrics.AllMetrics)
            {
                DatadogSeries metric = convertedMetrics[APP + "." + DOMAIN + "." + metricName + "." + timerName.GetDatadogName()];
                Assert.NotNull(metric);
                Assert.IsTrue(metric is DatadogGauge);
                ValidateDefaultTags(metric.Tags);
            }
        }

        [Test]
        public void SingleGaugeTest()
        {
            var metrics = new Metrics();
            string metricName = "GaugeMetric";
            GaugeMetric gaugeMetric = metrics.Gauge("test", metricName, GetNumberOfUsersLoggedIn);

            DataDogReporter reporter = CreateDefaultReporter(metrics);

            TestRequest request = new TestRequest();
            reporter.TransformMetrics(request, metrics, 0);

            IDictionary<string, DatadogSeries> convertedMetrics = request.Metrics;

            Assert.AreEqual(1, convertedMetrics.Count);
            DatadogSeries metric = convertedMetrics[APP + "." + DOMAIN + "." + metricName];
            Assert.NotNull(metric);
            Assert.IsTrue(metric is DatadogGauge);
            ValidateDefaultTags(metric.Tags);
        }

        private void ValidateDefaultTags(IList<string> tags)
        {
            Assert.AreEqual(2, tags.Count);
            foreach (string tag in tags) {
                if (!tag.Equals(DataDogReporter.ENVIRONMENT_TAG + ":" + ENVIRONMENT) && !tag.Equals(DataDogReporter.HOST_TAG + ":" + HOST)) 
                {
                    Assert.IsTrue(false);
                }
            }
        }

        private DataDogReporter CreateDefaultReporter(Metrics metrics)
        {
            ITransport transport = new UdpTransport.Builder().WithPort(8125)
                            .WithStatsdHost("appdev")
                            .Build();
            string[] path = { APP, DOMAIN };
            IMetricNameFormatter formatter = new AppendMetricNameToPathFormatter();
            var reporter = new DataDogReporter(metrics, transport, formatter, ENVIRONMENT, HOST, path);
            reporter.Start(5, TimeUnit.Seconds);
            return reporter;
        }

        private static long GetNumberOfUsersLoggedIn()
        {
            var rand = new Random();

            return rand.Next(2000);
        }
    }
}
