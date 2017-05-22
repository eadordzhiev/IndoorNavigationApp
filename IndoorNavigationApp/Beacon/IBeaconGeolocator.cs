using System.Collections.Generic;
using System.Threading.Tasks;
using IndoorNavigationApp.Models;

namespace IndoorNavigationApp.Beacon
{
    public interface IBeaconGeolocator
    {
        Task<BeaconGeolocation> FindGeolocationByBeaconData(BeaconData beaconData, Map map = null);
        IList<RouteSegment> MakeRoute(Building building, Node sourceNode, Node destinationNode);
    }
}