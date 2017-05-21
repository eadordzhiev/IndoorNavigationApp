using Newtonsoft.Json;

namespace IndoorNavigationApp.Models.Dto
{
    public class MapIdDto
    {

        [JsonProperty("mapId")]
        public int MapId { get; set; }
    }
}