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
        /// <summary>
        /// Build a request context
        /// </summary>
        /// <returns>The created request context</returns>
        IRequest Prepare();
    }
}
