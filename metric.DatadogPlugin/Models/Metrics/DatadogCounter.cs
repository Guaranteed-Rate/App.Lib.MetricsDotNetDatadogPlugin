using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Models.Metrics
{
    public class DatadogCounter : DatadogSeries
    {
        private readonly long count;
        public DatadogCounter(string name, long count, long epoch, string host, IList<string> additionalTags) 
            : base(name, epoch, host, additionalTags)
        {
            this.count = count;
        }

        public long GetCount()
        {
            return count;
        }
    }
}
