using System;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight;
using IndoorNavigationApp.Beacon;
using IndoorNavigationApp.Models;

namespace IndoorNavigationApp.ViewModels
{
    public class MapViewModel : ViewModelBase
    {
        public Map Map
        {
            get { return _map; }
            set
            {
                _map = value;
                RaisePropertyChanged();
            }
        }

        public PointU? CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                RaisePropertyChanged();
            }
        }

        public double HeadingNorth
        {
            get { return _headingNorth; }
            set
            {
                _headingNorth = value;
                RaisePropertyChanged();
            }
        }

        public bool IsCompassDataAvailable
        {
            get { return _isCompassDataAvailable; }
            set
            {
                _isCompassDataAvailable = value;
                RaisePropertyChanged();
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                RaisePropertyChanged();
            }
        }
        
        private readonly IBeaconWatcher _beaconWatcher;
        private readonly IBeaconGeolocator _beaconGeolocator;
        private readonly Compass _compass;
        private readonly DispatcherTimer _compassTimer;
        private Map _map;
        private PointU? _currentLocation;
        private double _headingNorth;
        private bool _isCompassDataAvailable;
        private bool _isLoading;

        public MapViewModel(IBeaconWatcher beaconWatcher, IBeaconGeolocator beaconGeolocator)
        {
            _beaconWatcher = beaconWatcher;
            _beaconGeolocator = beaconGeolocator;

            _compass = Compass.GetDefault();

            _compassTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(30) };
            _compassTimer.Tick += _compassTimer_Tick;
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
            _beaconWatcher.NearestBeaconChanged += NearestBeaconChangedHandler;
            _beaconWatcher.Start();

            switch (e.Parameter)
            {
                case Map map:
                    Map = map;
                    break;
                case BeaconGeolocation beaconGeolocation:
                    Map = beaconGeolocation.Map;
                    CurrentLocation = beaconGeolocation.NearestNode.Position;
                    break;
                default:
                    throw new ArgumentException();
            }

            if (_compass != null)
            {
                _compass.ReportInterval = 30;
                _compassTimer.Start();
            }
        }

        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            _beaconWatcher.Stop();
            _beaconWatcher.NearestBeaconChanged -= NearestBeaconChangedHandler;

            if (_compass != null)
            {
                _compassTimer.Stop();
            }
        }

        private void _compassTimer_Tick(object sender, object e)
        {
            var reading = _compass.GetCurrentReading();
            if (reading == null)
            {
                IsCompassDataAvailable = false;
                return;
            }

            IsCompassDataAvailable = true;
            HeadingNorth = reading.HeadingTrueNorth ?? reading.HeadingMagneticNorth;
        }

        private async void NearestBeaconChangedHandler(object sender, BeaconAdvertisement nearestBeacon)
        {
            if (Map == null || nearestBeacon?.BeaconData == null)
            {
                await App.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CurrentLocation = null;
                });
                return;
            }

            var beaconGeolocation = await _beaconGeolocator.FindGeolocationByBeaconData(nearestBeacon.BeaconData, Map).ConfigureAwait(false);
            if (beaconGeolocation == null)
            {
                await App.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    IsLoading = true;
                });
                try
                {
                    beaconGeolocation = await _beaconGeolocator.FindGeolocationByBeaconData(nearestBeacon.BeaconData).ConfigureAwait(false);
                }
                finally
                {
                    await App.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        IsLoading = true;
                    });
                }
            }

            await App.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (beaconGeolocation != null)
                {
                    Map = beaconGeolocation.Map;
                    CurrentLocation = beaconGeolocation.NearestNode.Position;
                }
                else
                {
                    CurrentLocation = null;
                }
            });
        }
    }
}
