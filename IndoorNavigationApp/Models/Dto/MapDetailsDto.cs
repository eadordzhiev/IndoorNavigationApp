using System.Collections.Generic;
using Newtonsoft.Json;

namespace IndoorNavigationApp.Models.Dto
{
    public class MapDetailsDto
    {

        [JsonProperty("directory")]
        public string Directory { get; set; }

        [JsonProperty("tilesDir")]
        public string TilesDir { get; set; }

        [JsonProperty("tilesXcount")]
        public uint TilesXcount { get; set; }

        [JsonProperty("tilesYcount")]
        public uint TilesYcount { get; set; }

        [JsonProperty("mapLevel")]
        public string MapLevel { get; set; }

        [JsonProperty("mapSizeX")]
        public uint MapSizeX { get; set; }

        [JsonProperty("mapSizeY")]
        public uint MapSizeY { get; set; }

        [JsonProperty("buildingId")]
        public int BuildingId { get; set; }

        [JsonProperty("beacons")]
        public IList<BeaconDto> Beacons { get; set; }

        [JsonProperty("nodes")]
        public IList<NodeDto> Nodes { get; set; }

        [JsonProperty("mapId")]
        public int MapId { get; set; }
    }
}