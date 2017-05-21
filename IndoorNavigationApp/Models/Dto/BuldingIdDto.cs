using Newtonsoft.Json;

namespace IndoorNavigationApp.Models.Dto
{
    public class BuldingIdDto
    {

        [JsonProperty("buildingId")]
        public int BuildingId { get; set; }
    }
}