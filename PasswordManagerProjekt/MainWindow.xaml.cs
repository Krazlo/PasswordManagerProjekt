using System.IO;
using System.Windows;
using System.Windows.Controls;
using PwM_Library;
using PwM_UI.Views;
using System.Threading;

namespace PwM_UI
{
    public partial class MainWindow : Window
    {
        public VaultsView VaultsView => VaultsTab.Content as VaultsView;
        private static string passwordManagerDir = Globals.passwordManagerDirectory;
        public MainWindow()
        {
            InitializeComponent();
            InitializeVaultsDirectory(passwordManagerDir);

            // Set initial view
            NavigateToVaults();

            // Setup navigation events
            vaultIconButton.MouseLeftButtonDown += (s, e) => NavigateToVaults();
            appIconButton.MouseLeftButtonDown += (s, e) => NavigateToApplications();
        }

        private void InitializeVaultsDirectory(string directoryName)
        {
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
        }

        private void NavigateToVaults()
        {
            VaultsTab.Visibility = Visibility.Visible;
            ApplicationsTab.Visibility = Visibility.Collapsed;
            MainTabControl.SelectedItem = VaultsTab;
        }

        private void NavigateToApplications()
        {
            VaultsTab.Visibility = Visibility.Collapsed;
            ApplicationsTab.Visibility = Visibility.Visible;
            MainTabControl.SelectedItem = ApplicationsTab;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddVault addVault = new AddVault();
            addVault.ShowDialog();
        }
    }
}