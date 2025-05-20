using PwM_UI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PwM_UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();

            // Set initial view
            NavigateToVaults();

            // Setup navigation events
            VaultsButton.Checked += (s, e) => NavigateToVaults();
            ApplicationsButton.Checked += (s, e) => NavigateToApplications();
        }

        private void NavigateToVaults()
        {
            VaultsTab.Visibility = Visibility.Visible;
            ApplicationsTab.Visibility = Visibility.Collapsed;
            MainTabControl.SelectedItem = VaultsTab;

            if (DataContext is MainWindowViewModel vm)
            {
                vm.ShowVaults = true;
                vm.ShowApplications = false;
            }
        }

        private void NavigateToApplications()
        {
            VaultsTab.Visibility = Visibility.Collapsed;
            ApplicationsTab.Visibility = Visibility.Visible;
            MainTabControl.SelectedItem = ApplicationsTab;
            
            if (DataContext is MainWindowViewModel vm)
            {
                vm.ShowVaults = false;
                vm.ShowApplications = true;
            }
        }
    }
}