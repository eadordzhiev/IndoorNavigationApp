using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using IndoorNavigationApp.ViewModels;

namespace IndoorNavigationApp.Views
{
    public sealed partial class MapView : Page
    {
        public MapViewModel VM { get; } = ViewModelLocator.MapViewModel;
        
        public MapView()
        {
            InitializeComponent();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            VM.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            VM.OnNavigatedFrom(e);
        }
    }
}
