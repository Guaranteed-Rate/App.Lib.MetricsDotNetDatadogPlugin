using metric.DatadogPlugin.Models.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Interfaces
{
    public interface ITransport
    {
        /**
        * Build a request context.
        */
        IRequest Prepare();


    }
}
