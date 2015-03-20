using metric.DatadogPlugin.Models.Metrics;
using StatsdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Models.Transport
{
    class UdpTransport : ITransport
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger("UdpTransport");
        private readonly IDictionary<string, long> lastSeenCounters = new Dictionary<string, long>();
        private readonly double sampleRate;

        private UdpTransport(string prefix, string statsdHost, int port, string[] globalTags, double sampleRate) {

            var dogStatsdConfig = new StatsdConfig
            {
                Prefix = prefix,
                StatsdServerName = statsdHost,
                StatsdPort = port
            };

            DogStatsd.Configure(dogStatsdConfig);
            this.sampleRate = sampleRate;
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
            return new DogstatsdRequest(lastSeenCounters, sampleRate);
        }

        public class DogstatsdRequest : IRequest
        {
            //private readonly StatsDClient statsdClient;
            private readonly IDictionary<string, long> lastSeenCounters;
            private readonly double sampleRate;

            public DogstatsdRequest(IDictionary<string, long> lastSeenCounters, double sampleRate)
            {
                //this.statsdClient = statsdClient;
                this.lastSeenCounters = lastSeenCounters;
                this.sampleRate = sampleRate;
            }

            /**
             * statsd has no notion of batch request, so gauges are pushed as they are received
             */
            public void AddGauge(DatadogGauge gauge)
            {
                if (gauge.getPoints().Count > 1)
                {
                    Log.Debug("Gauge " + gauge.getMetric() + " has more than one data point, " +
                        "will pick the first point only");
                }
                double value = gauge.getPoints().ElementAt(0).ElementAt(1);
                string[] tags = gauge.getTags().ToArray();
                DogStatsd.Gauge(gauge.getMetric(), value, sampleRate, tags);
                //DogStatsd.Gauge(_metricBaseName + metricName.Name, metric.ValueAsString, 1, tags);
            }

            /**
             * statsd has no notion of batch request, so counters are pushed as they are received
             */
            public void AddCounter(DatadogCounter counter)
            {
                if (counter.getPoints().Count > 1)
                {
                    Log.Debug("Counter " + counter.getMetric() + " has more than one data point, " +
                        "will pick the first point only");
                }
                long value = counter.getPoints().ElementAt(0).ElementAt(1);
                string[] tags = counter.getTags().ToArray();
                StringBuilder sb = new StringBuilder("");
                for (int i = tags.Length - 1; i >= 0; i--)
                {
                    sb.Append(tags[i]);
                    if (i > 0)
                    {
                        sb.Append(",");
                    }
                }

                string metric = counter.getMetric();
                string readonlyMetricsSeenName = metric + ":" + sb.ToString();
                long readonlyValue = value;
                if (lastSeenCounters.ContainsKey(readonlyMetricsSeenName))
                {
                    // If we've seen this counter before then calculate the difference
                    // by subtracting the new value from the old. StatsD expects a relative
                    // counter, not an absolute!
                    readonlyValue = Math.Max(0, value - lastSeenCounters[readonlyMetricsSeenName]);
                }
                // Store the last value we saw so that the next addCounter call can make
                // the proper relative value
                lastSeenCounters.Add(readonlyMetricsSeenName, value);

                DogStatsd.Counter(metric, readonlyValue, sampleRate, tags);
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
