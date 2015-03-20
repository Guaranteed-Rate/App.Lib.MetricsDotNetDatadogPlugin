﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin
{
    class TagsMerger
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger("TagsMerger");

        private static readonly string[] tagDelim = { ":" };
        /**
         *
         * @param tags1 list of tags, each tag should be in the format of "key:value"
         * @param tags2 list of tags, each tag should be in the format of "key:value"
         * @return merged tags list. If there is duplicated key, tags in tags2 will overwrite tags
         * in tags1, and tags in the back of the list will overwrite tags in the front of the list.
         */
        public static IList<string> MergeTags(IList<string> tags1, IList<string> tags2)
        {
            if (tags1 == null || tags1.Count == 0)
            {
                return tags2;
            }
            else if (tags2 == null || tags2.Count == 0)
            {
                return tags1;
            }

            IDictionary<string, string> map = new Dictionary<string, string>();
            foreach (string tag in tags1)
            {
                ParseTag(tag, map);
            }
            foreach (string tag in tags2)
            {
                ParseTag(tag, map);
            }

            IList<string> newTags = new List<string>();
            foreach (KeyValuePair<string, string> pair in map)
            {
                newTags.Add(pair.Key + ":" + pair.Value);
            }

            return newTags;
        }


        private static void ParseTag(string tag, IDictionary<string, string> map)
        {
            string[] strs = tag.Split(tagDelim, StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length != 2)
            {
                Log.Warn("Invalid tag: " + tag);
            }
            else
            {
                map.Add(strs[0], strs[1]);
            }
        }
    }
}
