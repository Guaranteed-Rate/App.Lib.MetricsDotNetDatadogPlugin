using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GuaranteedRate.Metric.DatadogPlugin
{
    public class InvalidMetricNameFormatException : Exception, ISerializable
    {
        public InvalidMetricNameFormatException()
        {
        }

        public InvalidMetricNameFormatException(string message)
            : base(message)
        {
        }

        public InvalidMetricNameFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected InvalidMetricNameFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}
