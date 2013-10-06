using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Collector.Models
{
    public class LogEntry
    {
        private JObject jsonData;

        public LogEntry(JObject jsonData)
        {
            this.jsonData = jsonData;
        }

        public LogEntry(DateTime timeStamp, string session, JToken data)
        {
            this.jsonData = new JObject();
            this.TimeStamp = timeStamp;
            this.Session = session;
            this.Data = data;
        }

        [JsonProperty(PropertyName = "t")]
        public DateTime TimeStamp
        {
            get
            {
                return DateTime.FromBinary(long.Parse(jsonData.Value<string>("t"), System.Globalization.NumberStyles.HexNumber));
            }
            private set
            {
                jsonData["t"] = value.ToBinary().ToString("X");
            }
        }

        [JsonProperty(PropertyName = "s")]
        public string Session
        {
            get
            {
                return jsonData.Value<string>("s");
            }
            private set
            {
                jsonData["s"] = value;
            }
        }

        [JsonProperty(PropertyName = "d")]
        public JToken Data
        {
            get
            {
                return jsonData["d"];
            }
            private set
            {
                jsonData["d"] = value;
            }
        }

        public static LogEntry FromJson(string json)
        {
            return new LogEntry(JObject.Parse(json));
        }
    }
}