using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Models.Metrics
{
    /**
     * Change here from the java version - java is using generics to allow DatadogCounter to store data as 
     * any numeric (in this case long or double) but C# inheritance is different.
     * 
     * For a first version using a less elegant soultion of having the implementing class handle the data.
     */
    public abstract class DatadogSeries
    {
        public string _name { get; private set; }
        public long _epoch { get; private set; }
        public string _host { get; private set; }
        public IList<string> _tags { get; private set; }

        // Expect the tags in the pattern
        // namespace.metricName[tag1:value1,tag2:value2,etc....]
        private readonly Regex _tagPattern = new Regex("([\\w\\.]+)\\[([\\w\\W]+)\\]");
        private readonly string[] _tagSplit = { "," };

        public DatadogSeries(string name, long epoch, string host, IList<string> additionalTags)
        {
            MatchCollection matcher = _tagPattern.Matches(name);
            this._tags = new List<string>();
            if (matcher.Count > 0)
            {
                this._name = matcher[1].Value;
                foreach (string t in matcher[2].Value.Split(_tagSplit, StringSplitOptions.None))
                {
                    this._tags.Add(t);
                }
            }
            else
            {
                this._name = name;
            }
            if (additionalTags != null && additionalTags.Count > 0)
            {
                foreach (string tag in additionalTags)
                {
                    this._tags.Add(tag);
                }
            }
            this._epoch = epoch;
            this._host = host;
        }

        public string GetHost()
        {
            return _host;
        }

        public string GetMetric()
        {
            return _name;
        }

        public IList<string> GetTags()
        {
            return _tags;
        }

        public long GetTimestamp()
        {
            return _epoch;
        }

    }
}
