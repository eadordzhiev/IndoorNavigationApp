using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using IndoorNavigationApp.ViewModels;

namespace IndoorNavigationApp.Views
{
    public sealed partial class MainView : Page
    {
        public MainViewModel VM { get; } = ViewModelLocator.MainViewModel;

        public MainView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            VM.OnNavigatedTo(e);
        }
    }
}
