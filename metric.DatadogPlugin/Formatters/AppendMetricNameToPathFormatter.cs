using metric.DatadogPlugin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace metric.DatadogPlugin.Formatters
{
    /// <summary>
    /// This formatter assumes the name is in the format:
    /// this.is.a.name[with:some][tag:stuff]
    /// It will append the values in name AFTER the path
    /// 
    /// ex:
    ///  name = "this.is.a.name[with:some][tag:stuff]"
    ///  path[] = { "path","dir","subdir"}
    ///  returns "path.dir.subdir.this.is.a.name[with:some][tag:stuff]"
    /// </summary>
    public class AppendMetricNameToPathFormatter : IMetricNameFormatter
    {
        public const string DEFAULT_SEPARATOR  = ".";
        private readonly string _separator;

        public AppendMetricNameToPathFormatter(string separator)
        {
            _separator = separator;
        }

        public AppendMetricNameToPathFormatter()
        {
            _separator = AppendMetricNameToPathFormatter.DEFAULT_SEPARATOR;
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
                sb.Append(piece).Append(_separator);
            }
            sb.Append(name);
            return sb.ToString();
        }
    }
}
