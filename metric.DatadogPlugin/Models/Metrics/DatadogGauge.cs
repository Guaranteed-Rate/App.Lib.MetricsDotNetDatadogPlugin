using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Models.Metrics
{
    public class DatadogGauge : DatadogSeries
    {
        private readonly double value;
        public DatadogGauge(string name, double value, long epoch, string host, IList<string> additionalTags) 
            : base(name, epoch, host, additionalTags)
        {
            this.value = value;
        }

        public double GetValue()
        {
            return value;
        }
        
    }
}
