using Newtonsoft.Json;

namespace IndoorNavigationApp.Models.Dto
{
    public class BuildingNameDto
    {

        [JsonProperty("buildingId")]
        public int BuildingId { get; set; }

        [JsonProperty("buildingName")]
        public string BuildingName { get; set; }

        [JsonProperty("matrix")]
        public string Matrix { get; set; }
    }
}
