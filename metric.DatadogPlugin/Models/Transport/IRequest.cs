using metric.DatadogPlugin.Models.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Models.Transport
{
    /**
    * A request for batching of metrics to be pushed to datadog.
    * The call order is expected to be:
    *    one or more of addGauge, addCounter -> send()
    */
    interface IRequest
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
        * Send the request to datadog
        */
        void Send();
    }
}
