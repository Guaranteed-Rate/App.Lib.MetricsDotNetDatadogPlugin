using GuaranteedRate.Metric.DatadogPlugin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * This formatter assumes the name is in the format:
 * this.is.a.name[with:some][tag:stuff]
 * It will append the values in name AFTER the path
 * 
 * ex:
 *  name = "this.is.a.name[with:some][tag:stuff]"
 *  path[] = { "path","dir","subdir"}
 *  returns "path.dir.subdir.this.is.a.name[with:some][tag:stuff]"
 */
namespace GuaranteedRate.Metric.DatadogPlugin.Formatters
{
    public class AppendMetricNameToPathFormatter : IMetricNameFormatter
    {
        public const string DEFAULT_SEPERATOR  = ".";
        private readonly string _seperator;

        public AppendMetricNameToPathFormatter(string seperator)
        {
            this._seperator = seperator;
        }

        public AppendMetricNameToPathFormatter()
        {
            this._seperator = AppendMetricNameToPathFormatter.DEFAULT_SEPERATOR;
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
                sb.Append(piece).Append(_seperator);
            }
            sb.Append(name);
            return sb.ToString();
        }
    }
}
