using System;
using IndoorNavigationApp.Models;

namespace IndoorNavigationApp.Beacon
{
    public sealed class BeaconAdvertisement
    {
        public BeaconData BeaconData { get; }

        public sbyte TxPower { get; }

        public short Rssi { get; }

        public DateTimeOffset Timestamp { get; }

        public BeaconAdvertisement(BeaconData beaconData, sbyte txPower, short rssi, DateTimeOffset timestamp)
        {
            BeaconData = beaconData;
            TxPower = txPower;
            Rssi = rssi;
            Timestamp = timestamp;
        }
    }
}