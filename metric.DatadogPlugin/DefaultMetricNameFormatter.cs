using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin
{
    public class DefaultMetricNameFormatter : IMetricNameFormatter
    {

        /**
         * This formatter assumes the name is in the format:
         * this.is.a.name[with][some][stuff]
         * It will append the values in path after the '.' seperated fields, and before the [] fields
         */
        public string Format(string name, params string[] path) {
            StringBuilder sb = new StringBuilder();
            string[] pattern = new string[1];
            pattern[0] = "[";
            string[] metricParts = name.Split(pattern, StringSplitOptions.None);
            sb.Append(metricParts[0]);

            if (path != null && path.Length > 0) { 
                foreach (string part in path) {
                    sb.Append('.').Append(part);
                }
            }
            for (int i = 1; i < metricParts.Length; i++) {
                sb.Append('[').Append(metricParts[i]);
            }
            return sb.ToString();
          }
    }
}
