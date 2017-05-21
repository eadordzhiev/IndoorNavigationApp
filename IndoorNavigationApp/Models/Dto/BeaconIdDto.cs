using Newtonsoft.Json;

namespace IndoorNavigationApp.Models.Dto
{
    public class BeaconIdDto
    {

        [JsonProperty("id")]
        public int BeaconId { get; set; }
    }
}