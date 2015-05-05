# App.Lib.MetricsDotNetDatadogPlugin

This is a .NET translation of the metrics to datadog java adapter (https://github.com/coursera/metrics-datadog)
Essentially this code is an adapter between Metrics.Net and Datadog.

The initial version supports UDP only, Http is commented out but will be finished in a later release.

## Usage

~~~scala

using System;
using System.Threading;
using GuaranteedRate.Metric.DatadogPlugin;
using metrics;
using metrics.Core;
using GuaranteedRate.Metric.DatadogPlugin.Models;
using GuaranteedRate.Metric.DatadogPlugin.Models.Transport;
using GuaranteedRate.Metric.DatadogPlugin.Interfaces;
using GuaranteedRate.Metric.DatadogPlugin.Formatters;
using System.Collections.Generic;


...
ITransport transport = new UdpTransport.Builder().WithPort(port)
    .WithStatsdHost("appdev")
    .Build();

string host = "hostName";
string environment = "testEnv";
string[] path = { "ApplicationName", "DomainName" };

IMetricNameFormatter formatter = new AppendMetricNameToPathFormatter();

var reporter = new DataDogReporter(metrics, transport, formatter, environment, host, path);
reporter.Start(5, TimeUnit.Seconds);
...

~~~~

## Nuget Info

*Metrics.net* and *DogStats-CSharp Client* are available as artifacts on nuget

## Contributing

Code, comments and corrections welcome!