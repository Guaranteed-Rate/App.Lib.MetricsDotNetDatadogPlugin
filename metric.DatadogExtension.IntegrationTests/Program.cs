using System;
using System.Threading;
using metric.DatadogPlugin;
using metrics;
using metrics.Core;
using metric.DatadogPlugin.Models;
using metric.DatadogPlugin.Models.Transport;
using metric.DatadogPlugin.Interfaces;
using metric.DatadogPlugin.Formatters;
using System.Collections.Generic;

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
                string environment = "testEnv";
                string[] path = { "ApplicationName", "DomainName" };
                //IMetricNameFormatter formatter = new AppendMetricNameToPathFormatter();
                IMetricNameFormatter formatter = new AppendMetricNameToPathFormatter();
                var reporter = new DataDogReporter(metrics, transport, formatter, environment, host, path);
                reporter.Start(5, TimeUnit.Seconds);

                CounterMetric counter = metrics.Counter("test", "CounterMetric");
                HistogramMetric histogramMetric = metrics.Histogram("test", "HistogramMetric");
                GaugeMetric gaugeMetric = metrics.Gauge("test", "GaugeMetric", GetNumberOfUsersLoggedIn);
                var rand = new Random(System.DateTime.Now.Millisecond);

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
