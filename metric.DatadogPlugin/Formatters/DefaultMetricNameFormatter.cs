using metric.DatadogPlugin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Formatters
{
    public class DefaultMetricNameFormatter : IMetricNameFormatter
    {

        /**
         * This formatter assumes the name is in the format:
         * this.is.a.name[with:some][tag:stuff]
         * It will append the values in path after the '.' seperated fields, and before the [] fields
         * 
         * ex:
         *  name = "this.is.a.name[with:some][tag:stuff]"
         *  path[] = { "path","dir","subdir"}
         *  returns "this.is.a.name.path.dir.subdir[with:some][tag:stuff]"
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
