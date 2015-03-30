using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Models.Metrics
{
    public class DatadogCounter : DatadogSeries
    {
        public long _value { get; private set; }
        public DatadogCounter(string name, long value, long epoch, IDictionary<string, string> additionalTags) 
            : base(name, epoch, additionalTags)
        {
            this._value = value;
        }

    }
}
