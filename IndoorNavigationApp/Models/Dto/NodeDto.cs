using Newtonsoft.Json;

namespace IndoorNavigationApp.Models.Dto
{
    public class NodeDto
    {

        [JsonProperty("nodeId")]
        public int NodeId { get; set; }

        [JsonProperty("coordX")]
        public uint CoordX { get; set; }

        [JsonProperty("coordY")]
        public uint CoordY { get; set; }

        [JsonProperty("mapId")]
        public int MapId { get; set; }

        [JsonProperty("nodeNumber")]
        public int NodeNumber { get; set; }
    }
}