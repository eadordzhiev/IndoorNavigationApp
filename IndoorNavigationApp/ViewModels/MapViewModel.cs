using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight;
using IndoorNavigationApp.Beacon;
using IndoorNavigationApp.Models;
using IndoorNavigationApp.Service;

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

        public IReadOnlyList<RouteSegment> RouteSegments
        {
            get { return _routeSegments; }
            set
            {
                _routeSegments = value;
                RaisePropertyChanged();
            }
        }

        private IReadOnlyList<RouteSegment> _routeSegments;

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
        private readonly IMapServiceClient _mapServiceClient;
        private readonly Compass _compass;
        private readonly DispatcherTimer _compassTimer;
        private Map _map;
        private PointU? _currentLocation;
        private double _headingNorth;
        private bool _isCompassDataAvailable;
        private bool _isLoading;

        public MapViewModel(IBeaconWatcher beaconWatcher, IBeaconGeolocator beaconGeolocator, IMapServiceClient mapServiceClient)
        {
            _beaconWatcher = beaconWatcher;
            _beaconGeolocator = beaconGeolocator;
            _mapServiceClient = mapServiceClient;

            _compass = Compass.GetDefault();

            _compassTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(30) };
            _compassTimer.Tick += _compassTimer_Tick;
        }

        public async void OnNavigatedTo(NavigationEventArgs e)
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

            var building = await _mapServiceClient.GetBuildingByBuildingIdAsync(2);
            var nodes = building.Maps.SelectMany(x => x.Nodes).ToImmutableDictionary(x => x.NavigationId);

            RouteSegments = _beaconGeolocator.MakeRoute(building, nodes[68], nodes[60]).ToArray();
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
