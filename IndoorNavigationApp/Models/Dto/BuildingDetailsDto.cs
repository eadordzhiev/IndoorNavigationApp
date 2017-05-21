using System.Collections.Generic;
using Newtonsoft.Json;

namespace IndoorNavigationApp.Models.Dto
{
    public class BuildingDetailsDto
    {

        [JsonProperty("buildingId")]
        public int BuildingId { get; set; }

        [JsonProperty("buildingName")]
        public string BuildingName { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("house")]
        public string House { get; set; }

        [JsonProperty("matrix")]
        public string Matrix { get; set; }

        [JsonProperty("maps")]
        public IList<MapIdDto> Maps { get; set; }
    }
}