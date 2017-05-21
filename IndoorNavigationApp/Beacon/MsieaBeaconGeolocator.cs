using System.Linq;
using System.Threading.Tasks;
using IndoorNavigationApp.Models;
using IndoorNavigationApp.Service;

namespace IndoorNavigationApp.Beacon
{
    public sealed class MsieaBeaconGeolocator : IBeaconGeolocator
    {
        private readonly IMapServiceClient _mapServiceClient;

        public MsieaBeaconGeolocator(IMapServiceClient mapServiceClient)
        {
            _mapServiceClient = mapServiceClient;
        }

        public async Task<BeaconGeolocation> FindGeolocationByBeaconData(BeaconData beaconData, Map map = null)
        {
            if (map == null)
            {
                var beaconId = await _mapServiceClient.GetBeaconIdByBeaconDataAsync(beaconData).ConfigureAwait(false);
                if (beaconId == null)
                {
                    return null;
                }

                map = await _mapServiceClient.GetMapByBeaconIdAsync(beaconId.Value).ConfigureAwait(false);
                if (map == null)
                {
                    return null;
                }
            }

            var nearestNode = map.Nodes.FirstOrDefault(x => x.BeaconData == beaconData);
            if (nearestNode == null)
            {
                return null;
            }

            return new BeaconGeolocation(
                map: map, 
                nearestNode: nearestNode,
                nearestBeacon: beaconData);
        }
    }
}