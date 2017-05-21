using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IndoorNavigationApp.Beacon;
using IndoorNavigationApp.Models;
using IndoorNavigationApp.Service;
using IndoorNavigationApp.Views;

namespace IndoorNavigationApp.ViewModels
{
    public sealed class MainViewModel : ViewModelBase
    {
        public ObservableCollection<BuildingName> Buildings
        {
            get => _buildings;
            set
            {
                _buildings = value;
                RaisePropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<ItemClickEventArgs> ShowFloorsCommand { get; }

        public RelayCommand FindMeCommand { get; }

        private readonly IMapServiceClient _mapServiceClient;
        private readonly IBeaconWatcher _beaconWatcher;
        private readonly IBeaconGeolocator _beaconGeolocator;
        private ObservableCollection<BuildingName> _buildings;
        private bool _isLoading;

        public MainViewModel(IMapServiceClient mapServiceClient, IBeaconWatcher beaconWatcher, IBeaconGeolocator beaconGeolocator)
        {
            _mapServiceClient = mapServiceClient;
            _beaconWatcher = beaconWatcher;
            _beaconGeolocator = beaconGeolocator;

            ShowFloorsCommand = new RelayCommand<ItemClickEventArgs>(ShowFloors);
            FindMeCommand = new RelayCommand(FindMe);

            _beaconWatcher.RefreshRate = 0.5;
            _beaconWatcher.Start();
        }

        public async void OnNavigatedTo(NavigationEventArgs e)
        {
            IsLoading = true;

            try
            {
                var buildings = await _mapServiceClient.GetBuildingNamesAsync();
                Buildings = new ObservableCollection<BuildingName>(buildings);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private static void ShowFloors(ItemClickEventArgs e)
        {
            var building = (BuildingName)e.ClickedItem;
            App.MainFrame.Navigate(typeof(BuildingView), building);
        }

        private async void FindMe()
        {
            IsLoading = true;

            try
            {
                var nearestBeacon = _beaconWatcher.NearestBeacon;
                if (nearestBeacon == null)
                {
                    return;
                }

                var beaconGeolocation = await _beaconGeolocator.FindGeolocationByBeaconData(nearestBeacon.BeaconData);
                if (beaconGeolocation == null)
                {
                    return;
                }

                App.MainFrame.Navigate(typeof(MapView), beaconGeolocation);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
