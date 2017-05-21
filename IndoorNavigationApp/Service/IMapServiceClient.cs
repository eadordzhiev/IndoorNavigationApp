using System.Collections.Generic;
using System.Threading.Tasks;
using IndoorNavigationApp.Models;

namespace IndoorNavigationApp.Service
{
    public interface IMapServiceClient
    {
        Task<int?> GetBeaconIdByBeaconDataAsync(BeaconData beaconId);
        Task<Building> GetBuildingDetailsAsync(int buildingId);
        Task<IReadOnlyList<BuildingName>> GetBuildingNamesAsync();
        Task<Map> GetMapByBeaconIdAsync(int beaconId);
        Task<Map> GetMapByIdAsync(int mapId);
    }
}