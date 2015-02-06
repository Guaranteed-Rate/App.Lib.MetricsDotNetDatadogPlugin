using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metric.DatadogPlugin.Models
{
    public class DataDogReporterConfigModel
    {
        public string DataDogAgentServerName { get; set; }
        public int DataDogListeningPort { get; set; }
        public string SourceApplicationName { get; set; }
        public string SourceDomainName { get; set; }
        public string SourceEnvironmentTag { get; set; }

        public DataDogReporterConfigModel(string dataDogAgentServerName, int dataDogListeningPort, string sourceApplicationName, string sourceDomainName, string sourceEnvironmentTag)
        {
            this.DataDogAgentServerName = dataDogAgentServerName;
            this.DataDogListeningPort = dataDogListeningPort;
            this.SourceApplicationName = sourceApplicationName;
            this.SourceDomainName = sourceDomainName;
            this.SourceEnvironmentTag = sourceEnvironmentTag;
        }
    }
}
