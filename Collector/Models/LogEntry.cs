using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Collector.Models
{
    class LogEntry
    {
        [JsonProperty(PropertyName = "t")]
        public string TimeStamp { get; set; }

        [JsonProperty(PropertyName = "s")]
        public string Session { get; set; }

        [JsonProperty(PropertyName = "d")]
        public JObject Data { get; set; }
    }
}