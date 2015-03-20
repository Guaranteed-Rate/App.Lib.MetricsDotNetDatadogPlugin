using metric.DatadogPlugin;
using metric.DatadogPlugin.Models;
using metric.DatadogPlugin.Models.Metrics;
using metric.DatadogPlugin.Models.Transport;
using metrics;
using metrics.Core;
using metrics.Reporting;
using metrics.Stats;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin
{
    class DatadogReporter2 : ReporterBase
    {

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger("DataDogReporter");

        /*
  private static readonly Expansion[] STATS_EXPANSIONS = { Expansion.MAX, Expansion.MEAN,
      Expansion.MIN, Expansion.STD_DEV, Expansion.MEDIAN, Expansion.P75, Expansion.P95,
      Expansion.P98, Expansion.P99, Expansion.P999 };
  private static readonly Expansion[] RATE_EXPANSIONS = { Expansion.RATE_1_MINUTE,
      Expansion.RATE_5_MINUTE, Expansion.RATE_15_MINUTE, Expansion.RATE_MEAN };
        */
        public sealed class Expansion
        {

            public static readonly Expansion COUNT = new Expansion("count");
            public static readonly Expansion RATE_MEAN = new Expansion("meanRate");
            public static readonly Expansion RATE_1_MINUTE = new Expansion("1MinuteRate");
            public static readonly Expansion RATE_5_MINUTE = new Expansion("5MinuteRate");
            public static readonly Expansion RATE_15_MINUTE = new Expansion("15MinuteRate");
            public static readonly Expansion MIN = new Expansion("min");
            public static readonly Expansion MEAN = new Expansion("mean");
            public static readonly Expansion MAX = new Expansion("max");
            public static readonly Expansion STD_DEV = new Expansion("stddev");
            public static readonly Expansion MEDIAN = new Expansion("median");
            public static readonly Expansion P75 = new Expansion("p75");
            public static readonly Expansion P95 = new Expansion("p95");
            public static readonly Expansion P98 = new Expansion("p98");
            public static readonly Expansion P99 = new Expansion("p99");
            public static readonly Expansion P999 = new Expansion("p999");

            public static readonly ISet<Expansion> ALL = new HashSet<Expansion> { COUNT, RATE_1_MINUTE, RATE_5_MINUTE, RATE_15_MINUTE, 
                                                                                    MIN, MEAN, MAX, STD_DEV,
                                                                                    MEDIAN, P75, P95, P98, P99, P999 };

            private readonly string name;
            private Expansion(string name)
            {
                this.name = name;
            }

            public override string ToString()
            {
                return name;
            }

        }


        private readonly ITransport transport;
        private readonly IDateTimeSupplier clock;
        private readonly string host;
        private readonly ISet<Expansion> expansions;
        private readonly IMetricNameFormatter metricNameFormatter;
        private readonly IList<string> tags;
        private readonly string prefix;
        private readonly IDynamicTagsCallback tagsCallback;
        private IRequest request;
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private readonly Metrics metrics;

        private DatadogReporter2(Metrics metricRegistry,
                          ITransport transport,
            //MetricFilter filter,
            //IDateTimeSupplier clock,
                          string host,
                          ISet<Expansion> expansions,
                          TimeUnit rateUnit,
                          TimeUnit durationUnit,
                          IMetricNameFormatter metricNameFormatter,
                          IList<string> tags,
                          string prefix,
                          IDynamicTagsCallback tagsCallback)
            : base(new TextMessageWriter(), metricRegistry)
        {
            //super(metricRegistry, "datadog-reporter", filter, rateUnit, durationUnit);
            this.metrics = metricRegistry;
            //this.clock = clock;
            this.host = host;
            this.expansions = expansions;
            this.metricNameFormatter = metricNameFormatter;
            this.tags = (tags == null) ? new List<string>() : tags;
            this.transport = transport;
            this.prefix = prefix;
            this.tagsCallback = tagsCallback;
        }


        public override void Run()
        {
            TimeSpan span = (clock.UtcNow - UnixEpoch);
            long timestamp = (int)Math.Floor(span.TotalSeconds);

            IList<string> newTags = tags;
            if (tagsCallback != null)
            {
                IList<string> dynamicTags = tagsCallback.GetTags();
                if (dynamicTags != null && dynamicTags.Count > 0)
                {
                    newTags = TagsMerger.MergeTags(tags, dynamicTags);
                }
            }

            try
            {
                request = transport.Prepare();

                foreach (var dictEntry in metrics.All)
                {
                    if (dictEntry.Value is CounterMetric)
                    {
                        ReportCounter(ApplyPrefix(dictEntry.Key.ToString()), (CounterMetric)dictEntry.Value, timestamp, newTags);
                    }
                    else if (dictEntry.Value is HistogramMetric)
                    {
                        ReportHistogram(ApplyPrefix(dictEntry.Key.ToString()), (HistogramMetric)dictEntry.Value, timestamp, newTags);

                    }
                    else if (dictEntry.Value is MeterMetric)
                    {
                        //        reportMetered(applyPrefix(entry.getKey()), entry.getValue(), timestamp, newTags);

                    }
                    else if (dictEntry.Value is TimerMetric)
                    {
                        //         reportTimer(applyPrefix(entry.getKey()), entry.getValue(), timestamp, newTags);

                    }
                    else if (dictEntry.Value is GaugeMetric)
                    {
                        ReportGauge(ApplyPrefix(dictEntry.Key.ToString()), (GaugeMetric)dictEntry.Value, timestamp, newTags);
                    }
                    else
                    {
                        Log.InfoFormat("Unknown metric type {}, not sending", dictEntry.Value.GetType());
                    }

                }

                request.Send();
            }
            catch (Exception e)
            {
                Log.Error("Error reporting metrics to Datadog: " + e);
            }
        }

        private void ReportTimer(string name, TimerMetric timer, long timestamp, IList<string> tags)
        {
            //Snapshot snapshot = timer.getSnapshot();
            /*
        double[] values = { snapshot.getMax(), snapshot.getMean(), snapshot.getMin(), snapshot.getStdDev(),
            snapshot.getMedian(), snapshot.get75thPercentile(), snapshot.get95thPercentile(), snapshot.get98thPercentile(),
            snapshot.get99thPercentile(), snapshot.get999thPercentile() };

        for (int i = 0; i < STATS_EXPANSIONS.length; i++) {
            if (expansions.contains(STATS_EXPANSIONS[i])) {
            request.addGauge(new DatadogGauge(
                appendExpansionSuffix(name, STATS_EXPANSIONS[i]),
                toNumber(convertDuration(values[i])),
                timestamp,
                host,
                tags));
            }
             */
        }

        private void ReportMetered(string name, MeterMetric meter, long timestamp, IList<string> tags)
        {
            /*
        if (expansions.contains(Expansion.COUNT)) {
            request.addCounter(new DatadogCounter(
                appendExpansionSuffix(name, Expansion.COUNT),
                meter.getCount(),
                timestamp,
                host,
                tags));
        }

        double[] values = { meter.getOneMinuteRate(), meter.getFiveMinuteRate(),
            meter.getFifteenMinuteRate(), meter.getMeanRate() };

        for (int i = 0; i < RATE_EXPANSIONS.length; i++) {
            if (expansions.contains(RATE_EXPANSIONS[i])) {
            request.addGauge(new DatadogGauge(
                appendExpansionSuffix(name, RATE_EXPANSIONS[i]),
                toNumber(convertRate(values[i])),
                timestamp,
                host,
                tags));
            }
        }
             */
        }

        private void ReportHistogram(string name, HistogramMetric histogram, long timestamp, IList<string> tags)
        {
            //Snapshot snapshot = histogram.;

            //if (expansions.Contains(Expansion.COUNT)) {
            request.AddCounter(new DatadogCounter(
                AppendExpansionSuffix(name, Expansion.COUNT),
                histogram.Count,
                timestamp,
                host,
                tags));
            //}
            /*
        long[] values = { histogram.getMax(), histogram.getMean(), histogram.getMin(), histogram.getStdDev(),
            histogram.getMedian(), histogram.get75thPercentile(), histogram.get95thPercentile(), histogram.get98thPercentile(),
            histogram. .Get99thPercentile(), histogram.get999thPercentile() };
        for (int i = 0; i < STATS_EXPANSIONS.length; i++) {
            if (expansions.contains(STATS_EXPANSIONS[i])) {
            request.addGauge(new DatadogGauge(
                appendExpansionSuffix(name, STATS_EXPANSIONS[i]),
                toNumber(values[i]),
                timestamp,
                host,
                tags));
            }
             *         */

        }

        private void ReportCounter(string name, CounterMetric counter, long timestamp, IList<string> tags)
        {
            request.AddCounter(new DatadogCounter(name, counter.Count, timestamp, host, tags));
        }

        private void ReportGauge(string name, GaugeMetric gauge, long timestamp, IList<string> tags)
        {
            long value = Convert.ToInt32(gauge.ValueAsString);
            
            if (value != null)
            {
                request.AddGauge(new DatadogGauge(name, value, timestamp, host, tags));
            }
        }

        private string AppendExpansionSuffix(string name, Expansion expansion)
        {
            return metricNameFormatter.Format(name, expansion.ToString());
        }

        private string ApplyPrefix(string name)
        {
            if (prefix == null)
            {
                return name;
            }
            else
            {
                return prefix + "." + name;
            }
        }


        public static Builder ForRegistry(Metrics registry)
        {
            return new Builder(registry);
        }

        public class Builder
        {
            private readonly Metrics registry;
            private string host;
            private ISet<metric.DatadogPlugin.DatadogReporter2.Expansion> expansions;
            private IDateTimeSupplier clock;
            private TimeUnit rateUnit;
            private TimeUnit durationUnit;
            //private MetricFilter filter;
            private IMetricNameFormatter metricNameFormatter;
            private List<String> tags;
            private ITransport transport;
            private string prefix;
            private IDynamicTagsCallback tagsCallback;

            public Builder(Metrics registry)
            {
                this.registry = registry;
                this.expansions = metric.DatadogPlugin.DatadogReporter2.Expansion.ALL;
                //this.clock = new DateTimeSupplier();
                //this.rateUnit = TimeUnit.SECONDS;
                //this.durationUnit = TimeUnit.MILLISECONDS;
                //this.filter = MetricFilter.ALL;
                this.metricNameFormatter = new DefaultMetricNameFormatter();
                this.tags = new List<string>();
            }

            public Builder WithHost(string host)
            {
                this.host = host;
                return this;
            }

            /*
        public Builder WithEC2Host() {
            this.host = AwsHelper.getEc2InstanceId();
            return this;
        }
             */

            public Builder WithExpansions(ISet<Expansion> expansions)
            {
                this.expansions = expansions;
                return this;
            }

            public Builder WithDynamicTagCallback(IDynamicTagsCallback tagsCallback)
            {
                this.tagsCallback = tagsCallback;
                return this;
            }

            public Builder convertRatesTo(TimeUnit rateUnit)
            {
                this.rateUnit = rateUnit;
                return this;
            }

            /**
                * Tags that would be sent to datadog with each and every metrics. This could be used to set
                * global metrics like version of the app, environment etc.
                * @param tags List of tags eg: [env:prod, version:1.0.1, name:kafka_client] etc
                */
            public Builder withTags(List<String> tags)
            {
                this.tags = tags;
                return this;
            }

            /**
                * Prefix all metric names with the given string.
                *
                * @param prefix The prefix for all metric names.
                */
            public Builder WithPrefix(string prefix)
            {
                this.prefix = prefix;
                return this;
            }

            public Builder WithMetricNameFormatter(IMetricNameFormatter formatter)
            {
                this.metricNameFormatter = formatter;
                return this;
            }

            public Builder convertDurationsTo(TimeUnit durationUnit)
            {
                this.durationUnit = durationUnit;
                return this;
            }

            /**
                * The transport mechanism to push metrics to datadog. Supports http webservice and UDP
                * dogstatsd protocol as of now.
                *
                * @see org.coursera.metrics.datadog.transport.HttpTransport
                * @see org.coursera.metrics.datadog.transport.UdpTransport
                */
            public Builder withTransport(ITransport transport)
            {
                this.transport = transport;
                return this;
            }

            public DatadogReporter2 build()
            {
                if (transport == null)
                {
                    throw new ArgumentException("Transport for datadog reporter is null. " +
                        "Please set a valid transport");
                }
                return new DatadogReporter2(
                    this.registry,
                    this.transport,
                    //this.clock,
                    this.host,
                    this.expansions,
                    this.rateUnit,
                    this.durationUnit,
                    this.metricNameFormatter,
                    this.tags,
                    this.prefix,
                    this.tagsCallback);
            }
        }
    }
}
    
