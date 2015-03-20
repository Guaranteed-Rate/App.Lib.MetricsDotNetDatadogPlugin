using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using metrics;
using metrics.Core;
using metric.DatadogPlugin.Models;

namespace metric.DatadogPlugin.Tests
{
    [TestClass]
    public class DataDogReporterTest
    {
        [TestMethod]
        public void SimpleTest()
        {
            var metrics = new Metrics();
            CounterMetric counter = metrics.Counter("test", "HealthMetrics.Test.SimpleCounter");
            DataDogReporterConfigModel dataDogReporterConfigModel = new DataDogReporterConfigModel("appdev", 8125, "ApplicationName", "DomainName", "Development");
            DataDogReporter reporter = new DataDogReporter(metrics, dataDogReporterConfigModel);

            //reporter.
            Assert.AreEqual(1, 1);
        }
    }
}
