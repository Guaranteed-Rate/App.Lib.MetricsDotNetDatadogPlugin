using GuaranteedRate.Metric.DatadogPlugin.Models.Metrics;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Metric.DatadogPlugin.Tests.Models.Metrics
{
    [TestFixture]
    public class DatadogSeriesTest
    {
        [Test]
        public void SplitNameAndTags()
        {
            IDictionary<string, string> tags = new Dictionary<string, string>();
            tags.Add("env","prod");
            tags.Add("version","1.0.0");

            DatadogCounter counter = new DatadogCounter(
                "test[tag1:value1,tag2:value2,tag3:value3]", 1L, 1234L, tags);
            IList<string> allTags = counter.Tags;

            Assert.AreEqual(5, allTags.Count);
            Assert.AreEqual("tag1:value1", allTags[0]);
            Assert.AreEqual("tag2:value2", allTags[1]);
            Assert.AreEqual("tag3:value3", allTags[2]);
            Assert.AreEqual("env:prod", allTags[3]);
            Assert.AreEqual("version:1.0.0", allTags[4]);
        }
    }
}
