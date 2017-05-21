using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using IndoorNavigationApp.Models;
using IndoorNavigationApp.ViewModels;

namespace IndoorNavigationApp.Views
{
    public sealed partial class BuildingView : Page
    {
        public BuildingViewModel VM { get; } = ViewModelLocator.BuildingViewModel;

        public BuildingView()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var building = (BuildingName) e.Parameter;
            await VM.SetBuildingAsync(building);
        }
    }
}
