using metric.DatadogPlugin.Formatters;
using metric.DatadogPlugin.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Tests.Formatters
{
    [TestFixture]
    public class AppendMetricNameToPathTest
    {
        [Test]
        public void SimpleAppendMetricNameToPath()
        {
            IMetricNameFormatter formatter = new AppendMetricNameToPathFormatter();
            Assert.AreEqual(null, formatter.Format(null, null));

            string name = "MetricName";
            Assert.AreEqual(name, formatter.Format(name, null));

            string[] path = { "domain", "app" };
            Assert.AreEqual("domain.app.MetricName", formatter.Format(name, path));
        }

        [Test]
        public void SeparatorAppendMetricNameToPath()
        {
            IMetricNameFormatter formatter = new AppendMetricNameToPathFormatter("-");
            string name = "MetricName";
            string[] path = { "domain", "app" };
            Assert.AreEqual("domain-app-MetricName", formatter.Format(name, path));
        }

    }
}
