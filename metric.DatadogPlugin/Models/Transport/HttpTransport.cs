//using metric.DatadogPlugin.Models.Metrics;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace metric.DatadogPlugin.Models.Transport
//{
//    class HttpTransport : ITransport
//    {
//        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger("HttpTransport");

//        private readonly static string BASE_URL = "https://app.datadoghq.com/api/v1";
//        private readonly string seriesUrl;
//        private readonly int connectTimeout;     // in milliseconds
//        private readonly int socketTimeout;      // in milliseconds

//        private HttpTransport(string apiKey, int connectTimeout, int socketTimeout)
//        {
//            this.seriesUrl = BASE_URL + "/series?api_key=" + apiKey;
//            this.connectTimeout = connectTimeout;
//            this.socketTimeout = socketTimeout;
//        }

//        public IRequest Prepare()
//        {
//            return new HttpRequest(this);
//        }

//        public void close()
//        {
//        }

//        public class HttpRequest : IRequest
//        {
//            protected readonly Serializer serializer;

//            protected readonly HttpTransport transport;

//            public HttpRequest(HttpTransport transport)
//            {
//                this.transport = transport;
//                serializer = new JsonSerializer();
//                serializer.startObject();
//            }

//            public void AddGauge(DatadogGauge gauge)
//            {
//                serializer.appendGauge(gauge);
//            }

//            public void AddCounter(DatadogCounter counter)
//            {
//                serializer.appendCounter(counter);
//            }

//            public void Send()
//            {
//                serializer.endObject();
//                string postBody = serializer.getAsString();
//                if (Log.IsDebugEnabled)
//                {
//                    StringBuilder sb = new StringBuilder();
//                    sb.Append("Sending HTTP POST request to ");
//                    sb.Append(this.transport.seriesUrl);
//                    sb.Append(", POST body is: \n");
//                    sb.Append(postBody);
//                    Log.Debug(sb.ToString());
//                }
//                long start = System.currentTimeMillis();
//                Response response = Post(this.transport.seriesUrl)
//                    .useExpectContinue()
//                    .connectTimeout(this.transport.connectTimeout)
//                    .socketTimeout(this.transport.socketTimeout)
//                    .bodyString(postBody, ContentType.APPLICATION_JSON)
//                    .execute();
//                long elapsed = System.currentTimeMillis() - start;

//                if (LOG.isDebugEnabled())
//                {
//                    HttpResponse httpResponse = response.returnResponse();
//                    StringBuilder sb = new StringBuilder();

//                    sb.Append("Sent metrics to Datadog: ");
//                    sb.Append("  Timing: ").append(elapsed).append(" ms\n");
//                    sb.Append("  Status: ").append(httpResponse.getStatusLine().getStatusCode()).append("\n");

//                    string content = EntityUtils.toString(httpResponse.getEntity(), "UTF-8");
//                    sb.Append("  Content: ").append(content);

//                    Log.Debug(sb.ToString());
//                }
//                else
//                {
//                    response.discardContent();
//                }
//            }

//            public static class Builder
//            {
//                string apiKey;
//                int connectTimeout = 5000;
//                int socketTimeout = 5000;

//                public Builder withApiKey(string key)
//                {
//                    this.apiKey = key;
//                    return this;
//                }

//                public Builder withConnectTimeout(int milliseconds)
//                {
//                    this.connectTimeout = milliseconds;
//                    return this;
//                }

//                public Builder withSocketTimeout(int milliseconds)
//                {
//                    this.socketTimeout = milliseconds;
//                    return this;
//                }

//                public HttpTransport build()
//                {
//                    return new HttpTransport(apiKey, connectTimeout, socketTimeout);
//                }
//            }
//        }
//    }
//}
