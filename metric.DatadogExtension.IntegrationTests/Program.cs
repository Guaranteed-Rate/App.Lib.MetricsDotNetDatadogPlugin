using System;
using System.Threading;
using metric.DatadogPlugin;
using metrics;
using metrics.Core;
using metric.DatadogPlugin.Models;

namespace metric.DatadogExtension.IntegrationTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var metrics = new Metrics();
            DataDogReporterConfigModel dataDogReporterConfigModel = new DataDogReporterConfigModel("appdev", 8125, "ApplicationName", "DomainName", "Development");
            var reporter = new DataDogReporter(metrics, dataDogReporterConfigModel);
            reporter.Start(5, TimeUnit.Seconds);

            CounterMetric counter = metrics.Counter("test", "HealthMetrics.Test.SimpleCounter");
            HistogramMetric histogramMetric = metrics.Histogram("test", "HealthMetrics.Test.HistogramMetrics");
            GaugeMetric gaugeMetric = metrics.Gauge("test", "HealthMetrics.Test.GaugeMetrics", GetNumberOfUsersLoggedIn);
            var rand = new Random(1);

            int runs = 0;
            while (runs < 1000)
            {
                counter.Increment();
                counter.Increment();
                counter.Increment();

                histogramMetric.Update(rand.Next(100));
                histogramMetric.Update(rand.Next(100));
                histogramMetric.Update(rand.Next(100));
                histogramMetric.Update(rand.Next(100));
                histogramMetric.Update(rand.Next(100));

                Thread.Sleep(5000);

               runs++;
            }
        }

        private static long GetNumberOfUsersLoggedIn()
        {
            var rand = new Random();

            return rand.Next(2000);
        }
    }
}
