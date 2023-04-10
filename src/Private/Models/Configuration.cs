using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RepoManager
{
    [Serializable]
    internal sealed class Configuration
    {
        public List<RepositoryContainer> Container { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Protocol Protocol { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Provider Provider { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Language Language { get; set; }
    }
}
