using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Metric.DatadogPlugin.Interfaces
{
    interface IDynamicTagsCallback
    {
        IList<string> GetTags();
    }
}
