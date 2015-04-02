using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Interfaces
{
    interface IDynamicTagsCallback
    {
        IList<string> GetTags();
    }
}
