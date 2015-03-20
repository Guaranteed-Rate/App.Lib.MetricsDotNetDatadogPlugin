using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin
{
    class DefaultMetricNameFormatter : IMetricNameFormatter
    {

        public string Format(string name, params string[] path) {
            StringBuilder sb = new StringBuilder();
            string[] pattern = new string[1];
            pattern[0] = "\\[";
            string[] metricParts = name.Split(pattern, StringSplitOptions.RemoveEmptyEntries);
            sb.Append(metricParts[0]);

            foreach (string part in path) {
                sb.Append('.').Append(part);
            }

            for (int i = 1; i < metricParts.Length; i++) {
                sb.Append('[').Append(metricParts[i]);
            }
            return sb.ToString();
          }
    }
}
