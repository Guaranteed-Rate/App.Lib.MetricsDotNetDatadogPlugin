using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin
{
    public class AppendMetricNameToPathFormatter : IMetricNameFormatter
    {
        public static readonly string DEFAULT_SEPERATOR  = ".";
        private readonly string seperator;

        public AppendMetricNameToPathFormatter(string seperator)
        {
            this.seperator = seperator;
        }

        public AppendMetricNameToPathFormatter()
        {
            this.seperator = AppendMetricNameToPathFormatter.DEFAULT_SEPERATOR;
        }

        public string Format(string name, params string[] path)
        {
            if (name == null || name.Length == 0)
            {
                return null;
            }
            if (path == null || path.Length == 0)
            {
                return name;
            }
            StringBuilder sb = new StringBuilder();

            foreach (string piece in path)
            {
                sb.Append(piece).Append(seperator);
            }
            sb.Append(name);
            return sb.ToString();
        }
    }
}
