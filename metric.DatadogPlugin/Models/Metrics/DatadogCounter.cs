using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Models.Metrics
{
    class DatadogCounter : DatadogSeries
    {
        public DatadogCounter(string name, long count, long epoch, string host, IList<string> additionalTags) 
            : base(name, count, epoch, host, additionalTags)
        {
        }

        /*
        protected string GetType()
        {
            return "counter";
        }
         */
    }
}
