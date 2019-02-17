using System;
using System.Net.Http;
using Newtonsoft.Json;
using WebApiProxy.Core.Models;
using WebApiProxy.Tasks.Models;
using WebApiProxy.Tasks.Templates;
using System.Linq;

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
            //Append web method as suffix if one web API has more than one web methods.
            foreach (var controllerDefinition in config.Metadata.Definitions)
            {
                var duplicatedList = controllerDefinition.ActionMethods.GroupBy(method => method.Name).Where(group => group.Count() > 1).ToList();
                foreach (var duplicatedItem in duplicatedList)
                {
                    foreach (var method in duplicatedItem)
                    {
                        method.Name = method.Name + method.Type;
                    }
                }
            }

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
                    var metadataStr = response.Content.ReadAsStringAsync().Result;
                    var metadata = JsonConvert.DeserializeObject<Metadata>(metadataStr);
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
