using System;
using Windows.Devices.Bluetooth;

namespace IndoorNavigationApp.Beacon
{
    public interface IBeaconWatcher
    {
        double RefreshRate { get; set; }

        BeaconAdvertisement NearestBeacon { get; }

        event EventHandler<BeaconAdvertisement> NearestBeaconChanged;
        event EventHandler<BluetoothError> Stopped;

        void Start();
        void Stop();
    }
}