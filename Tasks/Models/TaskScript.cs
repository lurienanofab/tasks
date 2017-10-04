using MongoDB.Bson;
using System;
using Newtonsoft.Json;

namespace Tasks.Models
{
    public class TaskScript
    {
        [JsonIgnore]
        public ObjectId Id { get; set; }

        [JsonProperty("id")]
        public string ScriptId { get; set; }

        [JsonProperty("source")]
        public string ScriptSource { get; set; }

        [JsonProperty("lastUpdate")]
        public DateTime LastUpdate { get; set; }
    }
}