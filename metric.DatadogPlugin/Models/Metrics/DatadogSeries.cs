using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Models.Metrics
{
    /**
     * Change here from the java version - java is using generics to allow DatadogCounter to store data as longs
     * and guages as ints.  However the technique doesn't directly translate to C# so I'm just having both use longs.
     * 
     * This is slightly (4 bytes) less memory efficient, but probably not an issue
     */
    public abstract class DatadogSeries
    {
        private readonly string name;
        private long epoch;
        private string host;
        private IList<string> tags;

        // Expect the tags in the pattern
        // namespace.metricName[tag1:value1,tag2:value2,etc....]
        private readonly Regex tagPattern = new Regex("([\\w\\.]+)\\[([\\w\\W]+)\\]");
        private readonly string[] tagSplit = { "\\," };

        public DatadogSeries(string name, long epoch, string host, IList<string> additionalTags)
        {
            MatchCollection matcher = tagPattern.Matches(name);
            this.tags = new List<string>();
            if (matcher.Count > 0)
            {
                this.name = matcher[1].Value;
                foreach (string t in matcher[2].Value.Split(tagSplit, StringSplitOptions.RemoveEmptyEntries))
                {
                    this.tags.Add(t);
                }
            }
            else
            {
                this.name = name;
            }
            if (additionalTags != null && additionalTags.Count > 0)
            {
                foreach (string tag in additionalTags)
                {
                    this.tags.Add(tag);
                }
            }
            this.epoch = epoch;
            this.host = host;
        }

        //@JsonInclude(Include.NON_NULL)
        public string GetHost()
        {
            return host;
        }

        public string GetMetric()
        {
            return name;
        }

        public IList<string> GetTags()
        {
            return tags;
        }

        public long GetTimestamp()
        {
            return epoch;
        }

        /*
  @Override
  public boolean equals(Object o) {
    if (this == o) return true;
    if (!(o instanceof DatadogSeries)) return false;

    DatadogSeries that = (DatadogSeries) o;

    if (!count.equals(that.count)) return false;
    if (!epoch.equals(that.epoch)) return false;
    if (!host.equals(that.host)) return false;
    if (!name.equals(that.name)) return false;
    if (!tags.equals(that.tags)) return false;

    return true;
  }

  @Override
  public int hashCode() {
    int result = name.hashCode();
    result = 31 * result + count.hashCode();
    result = 31 * result + epoch.hashCode();
    result = 31 * result + host.hashCode();
    result = 31 * result + tags.hashCode();
    return result;
  }

  @Override
  public String toString() {
    return "DatadogSeries{" +
        "name='" + name + '\'' +
        ", count=" + count +
        ", epoch=" + epoch +
        ", host='" + host + '\'' +
        ", tags=" + tags +
        '}';
  }
         */
    }
}
