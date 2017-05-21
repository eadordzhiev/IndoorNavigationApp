using Newtonsoft.Json;
using System;

namespace IndoorNavigationApp.Models.Dto
{
    public class BeaconDto
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("uuid")]
        public Guid Uuid { get; set; }

        [JsonProperty("minor")]
        public ushort Minor { get; set; }

        [JsonProperty("major")]
        public ushort Major { get; set; }

        [JsonProperty("coordX")]
        public uint CoordX { get; set; }

        [JsonProperty("coordY")]
        public uint CoordY { get; set; }

        [JsonProperty("nodeNumber")]
        public int NodeNumber { get; set; }
    }
}