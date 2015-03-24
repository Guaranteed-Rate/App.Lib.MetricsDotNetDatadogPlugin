using metrics;
using metrics.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Tests
{
    [TestClass]
    public class HistogramConversionTest
    {
        
        [TestMethod]
        public void HistogramPercentages()
        {
            Metrics metrics = new Metrics();
            HistogramMetric histogramMetric = metrics.Histogram("test", "HealthMetrics.Test.HistogramMetrics");

            for (int x = 1; x <= 100; x++) { 
                histogramMetric.Update(x);
            }

            double[] percentials = { 0.75, 0.99, 0.999 };

            double[] results = histogramMetric.Percentiles(percentials);

            Assert.IsNotNull(results);
            Assert.AreEqual(3, results.Length);

            Assert.AreEqual(75, results[0], 1);
            Assert.AreEqual(99, results[1], 1);
            Assert.AreEqual(99, results[2], 1);

        }

    }
}
