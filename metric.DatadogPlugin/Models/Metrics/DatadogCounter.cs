using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Metric.DatadogPlugin.Models.Metrics
{
    public class DatadogCounter : DatadogSeries
    {
        public long Value { get; private set; }

        public DatadogCounter(string name, long value, long epoch, IDictionary<string, string> additionalTags) 
            : base(name, epoch, additionalTags)
        {
            this.Value = value;
        }

    }
}
