using GuaranteedRate.Metric.DatadogPlugin.Interfaces;
using GuaranteedRate.Metric.DatadogPlugin.Models.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Metric.DatadogPlugin.Tests.Models.Transport
{
    public class TestRequest : IRequest
    {
        public IDictionary<string, DatadogSeries> Metrics { get; private set; }

        public TestRequest()
        {
            this.Metrics = new Dictionary<string, DatadogSeries>();
        }

        public void AddGauge(DatadogGauge gauge)
        {
            Metrics.Add(gauge.Name, gauge);
        }

        public void AddCounter(DatadogCounter counter)
        {
            Metrics.Add(counter.Name, counter);
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
