﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Models.Metrics
{
    class DatadogGauge : DatadogSeries
    {
        public DatadogGauge(string name, long count, long epoch, string host, IList<string> additionalTags) 
            : base(name, count, epoch, host, additionalTags)
        {
        }

        /*
        protected string GetType()
        {
            return "gauge";
        }
         */
    }
}