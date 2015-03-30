using metric.DatadogPlugin.Interfaces;
using metric.DatadogPlugin.Models.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Tests.Models.Transport
{
    public class TestRequest : IRequest
    {
        public IDictionary<string, DatadogSeries> _metrics { get; private set; }

        public TestRequest()
        {
            this._metrics = new Dictionary<string, DatadogSeries>();
        }

        public void AddGauge(DatadogGauge gauge)
        {
            _metrics.Add(gauge._name, gauge);
        }

        public void AddCounter(DatadogCounter counter)
        {
            _metrics.Add(counter._name, counter);
        }

        public void AddEvent(DatadogEvent datadogEvent)
        {
            throw new NotImplementedException();
        }

        public void Send()
        {
            throw new NotImplementedException();
        }
    }
}
