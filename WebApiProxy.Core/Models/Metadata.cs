using System.Collections.Generic;

namespace WebApiProxy.Core.Models
{
    public class Metadata
    {
        public string Host { get; set; }

        /// <summary>
        /// For dynamic host use.
        /// </summary>
        public string HostKey { get; set; }

        public IEnumerable<ControllerDefinition> Definitions { get; set; }

        public IEnumerable<ModelDefinition> Models { get; set; }

    }
}
