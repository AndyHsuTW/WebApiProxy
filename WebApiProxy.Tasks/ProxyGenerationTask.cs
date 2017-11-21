using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using WebApiProxy.Tasks.Infrastructure;
using WebApiProxy.Tasks.Models;

namespace WebApiProxy.Tasks
{
    public class ProxyGenerationTask : ITask
    {
        private List<Configuration> configList;

        [Output] 
        public string Filename { get; set; }

        [Output]
        public string Root { get; set; }

        [Output]
        public string ProjectPath { get; set; }

        public IBuildEngine BuildEngine { get; set; }

        public ITaskHost HostObject { get; set; }

        public bool Execute()
        {
            try
            {
                configList = Configuration.LoadList(Root);
                foreach (var config in configList)
                {
                    if (config.GenerateOnBuild)
                    {
                        var generator = new CSharpGenerator(config);
                        var source = generator.Generate();
                        File.WriteAllText(config.ConfigFileName, source);
                        File.WriteAllText(config.CacheFileName, source);
                    }
                }

            }
            catch (ConnectionException)
            {
                tryReadFromCache();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        private void tryReadFromCache()
        {
            if (configList != null)
            {
                foreach (var config in configList)
                {
                    if (!File.Exists(config.CacheFileName))
                    {
                        throw new ConnectionException(config.Endpoint);
                    }
                    var source = File.ReadAllText(config.CacheFileName);
                    File.WriteAllText(config.ConfigFileName, source);
                }
            }

        }
    }
}