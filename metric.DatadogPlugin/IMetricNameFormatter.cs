using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin
{
    public interface IMetricNameFormatter
    {
        string Format(string name, params string[] path);
    }
}
