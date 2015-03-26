using metric.DatadogPlugin.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Tests
{
    [TestClass]
    public class AppendMetricNameToPathTest
    {
        [TestMethod]
        public void SimpleAppendMetricNameToPath()
        {
            IMetricNameFormatter formatter = new AppendMetricNameToPathFormatter();
            Assert.AreEqual(null, formatter.Format(null, null));

            string name = "MetricName";
            Assert.AreEqual(name, formatter.Format(name, null));

            string[] path = { "domain", "app" };
            Assert.AreEqual("domain.app.MetricName", formatter.Format(name, path));
        }

        [TestMethod]
        public void SeperatorAppendMetricNameToPath()
        {
            IMetricNameFormatter formatter = new AppendMetricNameToPathFormatter("-");
            string name = "MetricName";
            string[] path = { "domain", "app" };
            Assert.AreEqual("domain-app-MetricName", formatter.Format(name, path));
        }

    }
}
