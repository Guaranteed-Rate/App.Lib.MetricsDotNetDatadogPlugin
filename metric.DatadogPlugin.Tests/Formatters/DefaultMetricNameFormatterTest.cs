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
    public class DefaultMetricNameFormatterTest
    {
        [Test]
        public void SimpleNameFormatTest()
        {
            IMetricNameFormatter formatter = new DefaultMetricNameFormatter();
            string name = "hi";
            string[] parameters = {"one"};
            string formatValue = formatter.Format(name, null);

            Assert.AreEqual(name, formatValue);

            formatValue = formatter.Format(name, parameters);
            Assert.AreEqual("hi.one", formatValue);
        }

        [Test]
        public void ComplexNameFormatTest()
        {
            IMetricNameFormatter formatter = new DefaultMetricNameFormatter();
            string name = "hi.how.are.you";
            string[] parameters = { "one", "two", "three" };
            string formatValue = formatter.Format(name, parameters);

            Assert.AreEqual("hi.how.are.you.one.two.three", formatValue);
        }

        [Test]
        public void BracketNameFormatTest()
        {
            IMetricNameFormatter formatter = new DefaultMetricNameFormatter();
            string name = "this.is.strange[hi][how][are][you]";
            string[] parameters = { "one", "two", "three" };
            string formatValue = formatter.Format(name, parameters);
            Assert.AreEqual("this.is.strange.one.two.three[hi][how][are][you]", formatValue);
        }
    }
}
