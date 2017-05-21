using System;
using System.Collections.Generic;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using Windows.System.Threading;
using IndoorNavigationApp.Models;
using MoreLinq;

namespace IndoorNavigationApp.Beacon
{
    public sealed class BeaconWatcher : IDisposable, IBeaconWatcher
    {
        public double RefreshRate
        {
            get { return _refreshRate; }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentException(nameof(value));
                }

                lock (_syncRoot)
                {
                    _refreshRate = value;
                    _expirationPeriod = TimeSpan.FromSeconds(1 / _refreshRate);
                }
            }
        }

        public BeaconAdvertisement NearestBeacon
        {
            get { return _nearestBeacon; }
            set
            {
                if (value == _nearestBeacon)
                {
                    return;
                }

                _nearestBeacon = value;
                OnNearestBeaconChanged(value);
            }
        }

        public event EventHandler<BeaconAdvertisement> NearestBeaconChanged;

        public event EventHandler<BluetoothError> Stopped;

        private readonly BluetoothLEAdvertisementWatcher _advertisementWatcher;
        private readonly List<BeaconAdvertisement> _advertisements = new List<BeaconAdvertisement>();
        private readonly object _syncRoot = new object();
        private ThreadPoolTimer _expirationTimer;
        private double _refreshRate;
        private TimeSpan _expirationPeriod;
        private BeaconAdvertisement _nearestBeacon;

        public BeaconWatcher()
        {
            RefreshRate = 1;

            _advertisementWatcher = new BluetoothLEAdvertisementWatcher
            {
                ScanningMode = BluetoothLEScanningMode.Passive
            };
            _advertisementWatcher.Received += AdvertisementWatcherReceivedHandler;
            _advertisementWatcher.Stopped += AdvertisementWatcherStoppedHandler;
        }

        public void Start()
        {
            lock (_syncRoot)
            {
                _advertisementWatcher.Start();

                if (_expirationTimer == null)
                {
                    _expirationTimer = ThreadPoolTimer.CreatePeriodicTimer(
                        handler: ExpirationTimerElapsedHandler,
                        period: TimeSpan.FromSeconds(0.1));
                }
            }
        }

        public void Stop()
        {
            lock (_syncRoot)
            {
                _advertisementWatcher.Stop();

                if (_expirationTimer != null)
                {
                    _expirationTimer.Cancel();
                    _expirationTimer = null;
                }
            }
        }

        private void ExpirationTimerElapsedHandler(ThreadPoolTimer timer)
        {
            lock (_syncRoot)
            {
                _advertisements.RemoveAll(x => DateTimeOffset.Now - x.Timestamp > _expirationPeriod);
                
                if (_advertisements.Count == 0)
                {
                    NearestBeacon = null;
                }
                else
                {
                    NearestBeacon = _advertisements.MaxBy(x => x.Rssi);
                }
            }
        }

        private void AdvertisementWatcherReceivedHandler(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            foreach (var manufacturerData in args.Advertisement.ManufacturerData)
            {
                const int appleCompanyId = 0x4c;
                if (manufacturerData.CompanyId != appleCompanyId)
                {
                    continue;
                }

                BeaconAdvertisement advertisement;
                using (var dataReader = DataReader.FromBuffer(manufacturerData.Data))
                {
                    var subtype = dataReader.ReadByte();
                    if (subtype != 2)
                    {
                        return;
                    }

                    var subtypeLength = dataReader.ReadByte();
                    if (subtypeLength != 0x15)
                    {
                        return;
                    }

                    var a = dataReader.ReadInt32();
                    var b = dataReader.ReadInt16();
                    var c = dataReader.ReadInt16();
                    var d = new byte[8];
                    dataReader.ReadBytes(d);
                    var uuid = new Guid(a, b, c, d);

                    var major = dataReader.ReadUInt16();
                    var minor = dataReader.ReadUInt16();
                    var txPower = (sbyte) dataReader.ReadByte();

                    advertisement = new BeaconAdvertisement(
                        beaconData: new BeaconData(
                            uuid: uuid,
                            major: major,
                            minor: minor), 
                        txPower: txPower,
                        rssi: args.RawSignalStrengthInDBm,
                        timestamp: args.Timestamp);
                }

                lock (_syncRoot)
                {
                    _advertisements.Add(advertisement);
                }
            }
        }

        private void AdvertisementWatcherStoppedHandler(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            Stop();
            OnStopped(args.Error);
        }

        void IDisposable.Dispose()
        {
            Stop();
        }

        private void OnNearestBeaconChanged(BeaconAdvertisement e)
        {
            NearestBeaconChanged?.Invoke(this, e);
        }

        private void OnStopped(BluetoothError e)
        {
            Stopped?.Invoke(this, e);
        }
    }
}