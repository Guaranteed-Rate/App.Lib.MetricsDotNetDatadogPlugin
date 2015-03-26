using System;
using System.Threading;
using metric.DatadogPlugin;
using metrics;
using metrics.Core;
using metric.DatadogPlugin.Models;
using metric.DatadogPlugin.Models.Transport;
using metric.DatadogPlugin.Interfaces;

namespace metric.DatadogExtension.IntegrationTests
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var metrics = new Metrics();
                //DataDogReporterConfigModel dataDogReporterConfigModel = new DataDogReporterConfigModel("appdev", 8125, "ApplicationName", "DomainName", "Development");

                ITransport transport = new UdpTransport.Builder().WithPort(8125)
                    .WithStatsdHost("appdev")
                    .Build();
                string host = "hostName";
                string[] path = { "ApplicationName", "DomainName" };
                //IMetricNameFormatter formatter = new AppendMetricNameToPathFormatter();
                IMetricNameFormatter formatter = new DefaultMetricNameFormatter();
                var reporter = new DataDogReporter(metrics, transport, formatter, host, path);
                reporter.Start(5, TimeUnit.Seconds);

                CounterMetric counter = metrics.Counter("test", "HealthMetrics.Test.SimpleCounter");
                HistogramMetric histogramMetric = metrics.Histogram("test", "HealthMetrics.Test.HistogramMetrics");
                GaugeMetric gaugeMetric = metrics.Gauge("test", "HealthMetrics.Test.GaugeMetrics", GetNumberOfUsersLoggedIn);
                var rand = new Random(1);

                int runs = 0;
                while (runs < 1000)
                {
                    System.Console.WriteLine("Loop " + (runs++) + " of 1000");
                    counter.Increment();
                    counter.Increment();
                    counter.Increment();

                    histogramMetric.Update(rand.Next(100));
                    histogramMetric.Update(rand.Next(100));
                    histogramMetric.Update(rand.Next(100));
                    histogramMetric.Update(rand.Next(100));
                    histogramMetric.Update(rand.Next(100));

                    Thread.Sleep(5000);

                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private static long GetNumberOfUsersLoggedIn()
        {
            var rand = new Random();

            return rand.Next(2000);
        }
    }
}
