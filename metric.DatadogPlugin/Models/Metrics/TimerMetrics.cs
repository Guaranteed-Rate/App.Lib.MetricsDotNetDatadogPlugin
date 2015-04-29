using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Reference class to help convert Metrics Histogram
 * names to Datadog style names
 */
namespace GuaranteedRate.Metric.DatadogPlugin.Models.Metrics
{
    public class TimerMetrics
    {
        public static readonly TimerMetrics Max = new TimerMetrics("max");
        public static readonly TimerMetrics Min = new TimerMetrics("min");
        public static readonly TimerMetrics Mean = new TimerMetrics("mean");
        public static readonly TimerMetrics StdDev = new TimerMetrics("stdDev");
        public static readonly TimerMetrics MeanRate = new TimerMetrics("meanRate");
        public static readonly TimerMetrics FifteenMinuteRate = new TimerMetrics("FifteenMinuteRate");
        public static readonly TimerMetrics FiveMinuteRate = new TimerMetrics("FiveMinuteRate");
        public static readonly TimerMetrics OneMinuteRate = new TimerMetrics("OneMinuteRate");

        public static readonly IList<TimerMetrics> AllMetrics = new List<TimerMetrics> { 
            Max,
            Min,
            Mean,
            StdDev,
            MeanRate,
            FifteenMinuteRate,
            FiveMinuteRate,
            OneMinuteRate
        };

        private string _name;
        TimerMetrics(string name)
        {
            this._name = name;
        }

        public string GetDatadogName() {
            return _name;
        }
    }
}
