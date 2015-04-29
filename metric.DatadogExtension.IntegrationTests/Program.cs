using System;
using System.Threading;
using GuaranteedRate.Metric.DatadogPlugin;
using metrics;
using metrics.Core;
using GuaranteedRate.Metric.DatadogPlugin.Models;
using GuaranteedRate.Metric.DatadogPlugin.Models.Transport;
using GuaranteedRate.Metric.DatadogPlugin.Interfaces;
using GuaranteedRate.Metric.DatadogPlugin.Formatters;
using System.Collections.Generic;

namespace GuaranteedRate.Metric.DatadogExtension.IntegrationTests
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
                var rand = new Random();

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
                throw;
            }
        }

        private static long GetNumberOfUsersLoggedIn()
        {
            var rand = new Random();
            return rand.Next(2000);
        }
    }
}
