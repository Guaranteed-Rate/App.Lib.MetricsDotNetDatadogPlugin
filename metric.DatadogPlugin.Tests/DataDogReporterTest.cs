using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using metrics.Core;
using metric.DatadogPlugin.Interfaces;
using metric.DatadogPlugin.Formatters;
using metrics;
using metric.DatadogPlugin.Models.Transport;
using metric.DatadogPlugin.Tests.Models.Transport;
using metric.DatadogPlugin.Models.Metrics;

namespace metric.DatadogPlugin.Tests
{
    [TestFixture]
    public class DataDogReporterTest
    {
        public const string host = "TestHost";
        public const string environment = "TestEnv";
        public const string app = "TestApp";
        public const string domain = "TestDomain";


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

            IDictionary<string, DatadogSeries> convertedMetrics = request._metrics;

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

            IDictionary<string, DatadogSeries> convertedMetrics = request._metrics;

            Assert.AreEqual(1, convertedMetrics.Count);
            DatadogSeries metric = convertedMetrics[app + "." + domain + "." + metricName];
            Assert.NotNull(metric);
            Assert.IsTrue(metric is DatadogCounter);
            ValidateDefaultTags(metric._tags);
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

            IDictionary<string, DatadogSeries> convertedMetrics = request._metrics;

            Assert.AreEqual(10, convertedMetrics.Count);
            foreach (HistogramMetrics histoName in HistogramMetrics.AllMetrics)
            {
                DatadogSeries metric = convertedMetrics[app + "." + domain + "." + metricName + "." + histoName.GetDatadogName()];
                Assert.NotNull(metric);
                Assert.IsTrue(metric is DatadogGauge);
                ValidateDefaultTags(metric._tags);
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

            IDictionary<string, DatadogSeries> convertedMetrics = request._metrics;

            Assert.AreEqual(8, convertedMetrics.Count);
            foreach (TimerMetrics timerName in TimerMetrics.AllMetrics)
            {
                DatadogSeries metric = convertedMetrics[app + "." + domain + "." + metricName + "." + timerName.GetDatadogName()];
                Assert.NotNull(metric);
                Assert.IsTrue(metric is DatadogGauge);
                ValidateDefaultTags(metric._tags);
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

            IDictionary<string, DatadogSeries> convertedMetrics = request._metrics;

            Assert.AreEqual(1, convertedMetrics.Count);
            DatadogSeries metric = convertedMetrics[app + "." + domain + "." + metricName];
            Assert.NotNull(metric);
            Assert.IsTrue(metric is DatadogGauge);
            ValidateDefaultTags(metric._tags);
        }

        private void ValidateDefaultTags(IList<string> tags)
        {
            Assert.AreEqual(2, tags.Count);
            foreach (string tag in tags) {
                if (!tag.Equals(DataDogReporter.ENVIRONMENT_TAG + ":" + environment) && !tag.Equals(DataDogReporter.HOST_TAG + ":" + host)) 
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
            string[] path = { app, domain };
            IMetricNameFormatter formatter = new AppendMetricNameToPathFormatter();
            var reporter = new DataDogReporter(metrics, transport, formatter, environment, host, path);
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
