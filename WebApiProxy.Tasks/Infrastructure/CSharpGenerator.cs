using System;
using System.Net.Http;
using WebApiProxy.Core.Models;
using WebApiProxy.Tasks.Models;
using WebApiProxy.Tasks.Templates;

namespace WebApiProxy.Tasks.Infrastructure
{
    public class CSharpGenerator
    {
        private readonly Configuration config;
        public CSharpGenerator(Configuration config)
        {
            this.config = config;
        }

        public string Generate()
        {
            config.Metadata = GetProxy();

            config.Metadata.HostKey = config.HostKey;
            config.Namespace = config.ConfigFileName.Replace(".config", "");
            if (String.IsNullOrEmpty(config.Metadata.HostKey))
            {
                config.Metadata.HostKey = "WebApiProxyHost";
            }
            var template = new CSharpProxyTemplate(config);
            var source = template.TransformText();
            return source;
        }


        private Metadata GetProxy()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-Proxy-Type", "metadata");
                    var response = client.GetAsync(config.Endpoint).Result;
                    response.EnsureSuccessStatusCode();
                    var metadata = response.Content.ReadAsAsync<Metadata>().Result;
                    return metadata;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
