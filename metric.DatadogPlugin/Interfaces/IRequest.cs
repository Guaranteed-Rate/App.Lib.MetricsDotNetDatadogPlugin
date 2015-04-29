using GuaranteedRate.Metric.DatadogPlugin.Models.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Metric.DatadogPlugin.Interfaces
{
    /**
    * A request for batching of metrics to be pushed to datadog.
    * The call order is expected to be:
    *    one or more of addGauge, addCounter -> send()
    */
    public interface IRequest
    {

        /**
        * Add a gauge
        */
        void AddGauge(DatadogGauge gauge);

        /**
        * Add a counter to the request
        */
        void AddCounter(DatadogCounter counter);

        /**
         * Add an event - this doesn't directly translate to a metric but it's useful 
         * functionality that is worth including.
         */
        void AddEvent(DatadogEvent datadogEvent);

        /**
        * Send the request to datadog
        */
        void Send();
    }
}
