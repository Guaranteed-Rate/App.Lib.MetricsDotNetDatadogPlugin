using GuaranteedRate.Metric.DatadogPlugin.Interfaces;
using GuaranteedRate.Metric.DatadogPlugin.Models.Metrics;
using StatsdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Metric.DatadogPlugin.Models.Transport
{
    public class UdpTransport : ITransport
    {
        /**
         * Datadog's native client expects counters to be sent as deltas from the previous value.
         * The RESTful http client expects counters to be sent as current value.
         */
        private readonly IDictionary<string, long> _lastSeenCounters = new Dictionary<string, long>();
        private readonly double _sampleRate;

        private UdpTransport(string prefix, string statsdHost, int port, string[] globalTags, double sampleRate) {

            var dogStatsdConfig = new StatsdConfig
            {
                Prefix = prefix,
                StatsdServerName = statsdHost,
                StatsdPort = port
            };

            DogStatsd.Configure(dogStatsdConfig);
            this._sampleRate = sampleRate;
        }

        public void close()
        {
            //statsd.stop();
        }

        public class Builder
        {
            string prefix = null;
            string statsdHost = "localhost";
            int port = 8125;
            double sampleRate = 1.0;

            public Builder WithPrefix(string prefix)
            {
                this.prefix = prefix;
                return this;
            }

            public Builder WithStatsdHost(string statsdHost)
            {
                this.statsdHost = statsdHost;
                return this;
            }

            public Builder WithPort(int port)
            {
                this.port = port;
                return this;
            }

            public UdpTransport Build()
            {
                return new UdpTransport(prefix, statsdHost, port, new string[0], sampleRate);
            }
        }

        public IRequest Prepare()
        {
            return new DogstatsdRequest(_lastSeenCounters, _sampleRate);
        }

        public class DogstatsdRequest : IRequest
        {
            private readonly IDictionary<string, long> _lastSeenCounters;
            private readonly double _sampleRate;

            public DogstatsdRequest(IDictionary<string, long> lastSeenCounters, double sampleRate)
            {
                this._lastSeenCounters = lastSeenCounters;
                this._sampleRate = sampleRate;
            }

            /**
             * statsd has no notion of batch request, so gauges are pushed as they are received
             */
            public void AddGauge(DatadogGauge gauge)
            {
                string[] tags = gauge.Tags.ToArray();
                DogStatsd.Gauge(gauge.Name, gauge.Value, _sampleRate, tags);
            }

            /**
             * statsd has no notion of batch request, so counters are pushed as they are received
             */
            public void AddCounter(DatadogCounter counter)
            {
                string[] tags = counter.Tags.ToArray();
                StringBuilder sb = new StringBuilder("");
                for (int i = tags.Length - 1; i >= 0; i--)
                {
                    sb.Append(tags[i]);
                    if (i > 0)
                    {
                        sb.Append(",");
                    }
                }

                string metric = counter.Name;
                string readonlyMetricsSeenName = metric + ":" + sb.ToString();
                long rawValue = counter.Value;
                long readonlyValue = rawValue;
                if (_lastSeenCounters.ContainsKey(readonlyMetricsSeenName))
                {
                    // If we've seen this counter before then calculate the difference
                    // by subtracting the new value from the old. StatsD expects a relative
                    // counter, not an absolute!
                    readonlyValue = Math.Max(0, rawValue - _lastSeenCounters[readonlyMetricsSeenName]);
                    _lastSeenCounters.Remove(readonlyMetricsSeenName);
                }
                // Store the last value we saw so that the next addCounter call can make
                // the proper relative value
                _lastSeenCounters.Add(readonlyMetricsSeenName, rawValue);

                DogStatsd.Counter(metric, readonlyValue, _sampleRate, tags);
            }

            public void AddEvent(DatadogEvent datadogEvent) {
                //TODO: TBD
                //DogStatsd.Event(title, text, alertType, aggregationKey, sourceType, dateHappened, priority, hostname, tags);
            }

            /**
             * For statsd the metrics are pushed as they are received. So there is nothing do in send.
             */
            public void Send()
            {
            }
        }

    }
}
