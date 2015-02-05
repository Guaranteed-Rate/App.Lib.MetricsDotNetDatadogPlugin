using System;
using System.Threading;
using metric.DatadogPlugin;
using metrics;
using metrics.Core;

namespace metric.DatadogExtension.IntegrationTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var metrics = new Metrics();
            //metrics.EnableDataDogReporting(5, TimeUnit.Seconds, "appdev", 8125);

            var reporter = new DataDogReporter(metrics, "appdev", 8125, "ilch1enap04d");
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

    //public class DatadogReporterTests
    //{
    //    [Test]
    //    public void CounterTest()
    //    {
    //        var metrics = new Metrics();
    //        //metrics.EnableDataDogReporting(5, TimeUnit.Seconds, "appdev", 8125);

    //        CounterMetric counter = metrics.Counter("test", "HealthMetrics.Test.SimpleCounter");
    //        HistogramMetric histogramMetric = metrics.Histogram("test", "HealthMetrics.Test.HistogramMetrics");
    //        GaugeMetric gaugeMetric = metrics.Gauge("test", "HealthMetrics.Test.GaugeMetrics", GetNumberOfUsersLoggedIn);
    //        var rand = new Random(1);

    //        int runs = 0;
    //        while (runs < 20)
    //        {
    //            counter.Increment();
    //            counter.Increment();
    //            counter.Increment();

    //            histogramMetric.Update(rand.Next(100));
    //            histogramMetric.Update(rand.Next(100));
    //            histogramMetric.Update(rand.Next(100));
    //            histogramMetric.Update(rand.Next(100));
    //            histogramMetric.Update(rand.Next(100));

    //            Thread.Sleep(5000);

    //            runs++;
    //        }
    //    }

    //    [Test]
    //    public void TimerTest()
    //    {
    //        var metrics = new Metrics();
    //        metrics.EnableDataDogReporting(5, TimeUnit.Seconds, "appdev", 8125);

    //        HistogramMetric histogramMetric = metrics.Histogram("test", "HealthMetrics.Test.HistogramMetrics");
    //        MeterMetric meter = metrics.Meter("test", "test meter", "test", TimeUnit.Seconds);

    //        var timer = new TimerMetric(TimeUnit.Seconds, TimeUnit.Seconds, meter, histogramMetric, false);
    //        for (int i = 0; i < 5; i++)
    //        {
    //            timer.Time(() =>
    //            {
    //                var randomTimer = new Random();
    //                Thread.Sleep(randomTimer.Next(5) * 1000);
    //            });
    //        }
    //    }

    //    private long GetNumberOfUsersLoggedIn()
    //    {
    //        var rand = new Random();

    //        return rand.Next(2000);
    //    }
    //}
}
