using Microsoft.Practices.ServiceLocation;

namespace IndoorNavigationApp.ViewModels
{
    public static class ViewModelLocator
    {
        public static MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>();

        public static BuildingViewModel BuildingViewModel => ServiceLocator.Current.GetInstance<BuildingViewModel>();

        public static MapViewModel MapViewModel => ServiceLocator.Current.GetInstance<MapViewModel>();
    }
}
