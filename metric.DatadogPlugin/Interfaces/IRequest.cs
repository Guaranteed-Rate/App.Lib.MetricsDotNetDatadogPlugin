using metric.DatadogPlugin.Models.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Interfaces
{
    /// <summary>
    /// A request for batching of metrics to be pushed to datadog.
    /// The call order is expected to be:
    /// one or more of addGauge, addCounter -> send()
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Add a gauge to the request
        /// </summary>
        /// <param name="gauge">The gauge to add</param>
        void AddGauge(DatadogGauge gauge);

        /// <summary>
        /// Add a counter to the request
        /// </summary>
        /// <param name="counter">The counter to add</param>
        void AddCounter(DatadogCounter counter);

        /// <summary>
        /// Add an event to the request - this doesn't directly translate to a metric but it's useful 
        /// functionality that is worth including.
        /// </summary> 
        /// <param name="datadogEvent">The event to add</param>
        void AddEvent(DatadogEvent datadogEvent);

        /// <summary>
        /// Send the request to DataDog
        /// </summary>
        void Send();
    }
}
