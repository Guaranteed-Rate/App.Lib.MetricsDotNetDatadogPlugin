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
         * This formatter doesn't make a lot of sense, it will insert the path values '.' seperated
         * between the first and second \\[ seperated block in the name.
         * BUT - it will also remove the first \\[ from the name.
         *  ex: name = "\\[This]\\[is]\\[strange], path = {"some","path"}
         *  becomes "This]some.path[is][strange]"
         *  
         * This is copied from the java version, but the java version doesn't have a test, so this could
         * be a bug in the original...I'll reach out to the original dev
         */
        public string Format(string name, params string[] path) {
            StringBuilder sb = new StringBuilder();
            string[] pattern = new string[1];
            pattern[0] = "\\[";
            string[] metricParts = name.Split(pattern, StringSplitOptions.RemoveEmptyEntries);
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
