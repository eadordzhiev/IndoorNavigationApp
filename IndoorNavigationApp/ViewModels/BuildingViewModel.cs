using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IndoorNavigationApp.Models;
using IndoorNavigationApp.Service;
using IndoorNavigationApp.Views;

namespace IndoorNavigationApp.ViewModels
{
    public sealed class BuildingViewModel : ViewModelBase
    {
        public string Title
        {
            get => _title;
            set
            {
                if (_title == value)
                {
                    return;
                }

                _title = value;
                RaisePropertyChanged();
            }
        }

        public Uri HeroImageUri
        {
            get => _heroImageUri;
            set
            {
                _heroImageUri = value;
                RaisePropertyChanged();
            }
        }

        public Address BuildingAddress
        {
            get { return _buildingAddress; }
            set
            {
                _buildingAddress = value;
                RaisePropertyChanged();
            }
        }
        
        public ObservableCollection<Map> Maps
        {
            get => _maps;
            set
            {
                if (_maps == value)
                {
                    return;
                }

                _maps = value;
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
        
        public RelayCommand<ItemClickEventArgs> ShowMapCommand { get; }
        
        private readonly IMapServiceClient _mapServiceClient;
        private string _title;
        private Uri _heroImageUri;
        private Address _buildingAddress;
        private ObservableCollection<Map> _maps;
        private bool _isLoading;

        public BuildingViewModel(IMapServiceClient mapServiceClient)
        {
            _mapServiceClient = mapServiceClient;

            ShowMapCommand = new RelayCommand<ItemClickEventArgs>(ShowMap);
        }

        public async Task SetBuildingAsync(BuildingName buildingName)
        {
            IsLoading = true;

            try
            {
                var building = await _mapServiceClient.GetBuildingDetailsAsync(buildingName.Id);

                Title = building.Name;
                HeroImageUri = building.HeroImageUri;
                Maps = new ObservableCollection<Map>(building.Maps);
                BuildingAddress = building.Address;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private static void ShowMap(ItemClickEventArgs e)
        {
            var map = (Map) e.ClickedItem;
            App.MainFrame.Navigate(typeof(MapView), map);
        }
    }
}
