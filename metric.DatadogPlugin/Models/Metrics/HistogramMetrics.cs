using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Reference class to help convert Metrics Histogram
 * names to Datadog style names
 */
namespace metric.DatadogPlugin.Models.Metrics
{
    public class HistogramMetrics
    {
        public static readonly HistogramMetrics Max = new HistogramMetrics("max");
        public static readonly HistogramMetrics Min = new HistogramMetrics("min");
        public static readonly HistogramMetrics Mean = new HistogramMetrics("mean");
        public static readonly HistogramMetrics StdDev = new HistogramMetrics("stdDev");
        public static readonly HistogramMetrics Count = new HistogramMetrics("count");
        public static readonly HistogramMetrics At75thPercentile = new HistogramMetrics("75percentile");
        public static readonly HistogramMetrics At95thPercentile = new HistogramMetrics("95percentile");
        public static readonly HistogramMetrics At98thPercentile = new HistogramMetrics("98percentile");
        public static readonly HistogramMetrics At99thPercentile = new HistogramMetrics("99percentile");
        public static readonly HistogramMetrics At999thPercentile = new HistogramMetrics("999percentile");

        public static readonly IList<HistogramMetrics> AllMetrics = new List<HistogramMetrics> { 
            Max,
            Min,
            Mean,
            StdDev,
            Count,
            At75thPercentile,
            At95thPercentile,
            At98thPercentile,
            At99thPercentile,
            At999thPercentile
        };

        private string _name;
        HistogramMetrics(string name)
        {
            this._name = name;
        }

        public string GetDatadogName() {
            return _name;
        }
    }
}
