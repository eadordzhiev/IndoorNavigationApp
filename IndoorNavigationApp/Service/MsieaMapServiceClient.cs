using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using IndoorNavigationApp.Models;
using IndoorNavigationApp.Models.Dto;
using Newtonsoft.Json;

namespace IndoorNavigationApp.Service
{
    public sealed class MsieaMapServiceClient : IMapServiceClient
    {
        private readonly HttpClient _httpClient;

        public MsieaMapServiceClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://miem-msiea.rhcloud.com")
            };
        }

        public async Task<IReadOnlyList<BuildingName>> GetBuildingNamesAsync()
        {
            var response = await _httpClient.GetStringAsync("/json?action=getBuildingsNames").ConfigureAwait(false);
            var buildingNameDtos = JsonConvert.DeserializeObject<BuildingNameDto[]>(response);
            return buildingNameDtos.Select(x => new BuildingName(x.BuildingId, x.BuildingName)).ToArray();
        }

        public async Task<Building> GetBuildingByBuildingIdAsync(int buildingId)
        {
            var response = await _httpClient.GetStringAsync($"/json?action=getBuildingInfo&buildingId={buildingId}").ConfigureAwait(false);
            var buildingDetailsDto = JsonConvert.DeserializeObject<BuildingDetailsDto>(response);

            var maps = new List<Map>();
            foreach (var mapIdDto in buildingDetailsDto.Maps)
            {
                maps.Add(await GetMapByIdAsync(mapIdDto.MapId).ConfigureAwait(false));
            }

            var adjacencyMatrix = await GetAdjacencyMatrixFromBuildingDetailsDto(buildingDetailsDto);

            return new Building(
                id: buildingDetailsDto.BuildingId,
                name: buildingDetailsDto.BuildingName,
                heroImageUri: new Uri($"ms-appx:/Assets/BuildingHeroImages/{buildingDetailsDto.BuildingName}.jpg"),
                maps: maps,
                address: new Address(
                    country: buildingDetailsDto.Country,
                    city: buildingDetailsDto.City,
                    street: buildingDetailsDto.Street,
                    house: buildingDetailsDto.House),
                adjacencyMatrix: adjacencyMatrix);
        }

        public async Task<Map> GetMapByIdAsync(int mapId)
        {
            var response = await _httpClient.GetStringAsync($"/json?action=getMapInfo&mapId={mapId}").ConfigureAwait(false);
            var mapDetailsDto = JsonConvert.DeserializeObject<MapDetailsDto>(response);
            return GetMapFromMapDetailsDto(mapDetailsDto);
        }

        public async Task<int?> GetBeaconIdByBeaconDataAsync(BeaconData beaconId)
        {
            var response = await _httpClient.GetAsync($"/json?action=getBeaconId&uuid={beaconId.Uuid}&major={beaconId.Major}&minor={beaconId.Minor}").ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var beaconIdDto = JsonConvert.DeserializeObject<BeaconIdDto>(responseString);
            return beaconIdDto.BeaconId;
        }
        
        public async Task<Map> GetMapByBeaconIdAsync(int beaconId)
        {
            var response = await _httpClient.GetAsync($"/json?action=getMapId&beaconId={beaconId}").ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var mapIdDto = JsonConvert.DeserializeObject<MapIdDto>(responseString);
            return await GetMapByIdAsync(mapIdDto.MapId);
        }

        private Node GetNodeFromBeaconDto(BeaconDto beaconDto)
        {
            NodeType nodeType;
            switch (beaconDto.Title)
            {
                case "S":
                    nodeType = NodeType.Stairs;
                    break;
                case "WC":
                    nodeType = NodeType.WC;
                    break;
                case "E":
                    nodeType = NodeType.Elevator;
                    break;
                case "L":
                case "L1":
                case "L2":
                    nodeType = NodeType.Library;
                    break;
                default:
                    nodeType = NodeType.GenericRoom;
                    break;
            }

            return new Node(
                id: beaconDto.Id,
                title: beaconDto.Title,
                description: beaconDto.Description,
                beaconData: new BeaconData(
                    uuid: beaconDto.Uuid,
                    major: beaconDto.Major,
                    minor: beaconDto.Minor), 
                type: nodeType,
                position: new PointU(beaconDto.CoordX, beaconDto.CoordY),
                navigationId: beaconDto.NodeNumber);
        }

        private Node GetNodeFromNodeDto(NodeDto nodeDto)
        {
            return new Node(
                id: nodeDto.NodeId,
                title: null,
                description: null,
                beaconData: null,
                type: NodeType.NavHint,
                position: new PointU(nodeDto.CoordX, nodeDto.CoordY),
                navigationId: nodeDto.NodeNumber);
        }

        private Map GetMapFromMapDetailsDto(MapDetailsDto mapDetailsDto)
        {
            var beaconNodes = mapDetailsDto.Beacons.Select(GetNodeFromBeaconDto);
            var navHintNodes = mapDetailsDto.Nodes.Select(GetNodeFromNodeDto);

            return new Map(
                id: mapDetailsDto.MapId,
                title: mapDetailsDto.MapLevel,
                size: new SizeU(mapDetailsDto.MapSizeX, mapDetailsDto.MapSizeY), 
                nodes: beaconNodes.Concat(navHintNodes).ToArray(),
                lowResolutionMapUri: new Uri(mapDetailsDto.Directory),
                tileProvider: new MsieaTileProvider(mapDetailsDto));
        }

        private async Task<int[,]> GetAdjacencyMatrixFromBuildingDetailsDto(BuildingDetailsDto buildingDetailsDto)
        {
            string adjacencyMatrixBody;
            if (buildingDetailsDto.BuildingName == "Strogino")
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/AdjacencyMatrices/strogino.txt"));
                adjacencyMatrixBody = await FileIO.ReadTextAsync(file);
            }
            else
            {
                adjacencyMatrixBody = await _httpClient.GetStringAsync(buildingDetailsDto.Matrix);
            }
            
            var rows = adjacencyMatrixBody.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var adjacencyMatrix = new int[rows.Length, rows.Length];
            for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
            {
                var columns = rows[rowIndex].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int columnIndex = 0; columnIndex < columns.Length; columnIndex++)
                {
                    adjacencyMatrix[rowIndex, columnIndex] = int.Parse(columns[columnIndex]);
                }
            }

            return adjacencyMatrix;
        }
    }
}
