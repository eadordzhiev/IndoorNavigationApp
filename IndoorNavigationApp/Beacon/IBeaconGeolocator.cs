using System.Threading.Tasks;
using IndoorNavigationApp.Models;

namespace IndoorNavigationApp.Beacon
{
    public interface IBeaconGeolocator
    {
        Task<BeaconGeolocation> FindGeolocationByBeaconData(BeaconData beaconData, Map map = null);
    }
}