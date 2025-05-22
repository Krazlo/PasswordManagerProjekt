using System.IO;
using System.Windows;
using System.Windows.Controls;
using PwM_Library;
using PwM_UI.Views;
using System.Threading;
using PwM_UI.Utility;
using System.ComponentModel;
using System.Windows.Media;
using System;
using System.Windows.Input;

namespace PwM_UI
{
    public partial class MainWindow : Window
    {
        public VaultsView VaultsView => VaultsTab.Content as VaultsView;
        public ApplicationsView ApplicationsView => ApplicationsTab.Content as ApplicationsView;
        public ApplicationsView AppsView => ApplicationsTab.Content as ApplicationsView;
        private static string passwordManagerDir = Globals.passwordManagerDirectory;
        public MainWindow()
        {
            InitializeComponent();
            InitializeVaultsDirectory(passwordManagerDir);

            // Access the VaultsView and hook up the callback
            if (VaultsTab.Content is VaultsView view)
            {
                view.OnVaultOpened = VaultOpenedHandler;
            }

            // Set initial view
            NavigateToVaults();

            // Setup navigation events
            vaultIconButton.MouseLeftButtonDown += (s, e) => NavigateToVaults();
        }

        private void VaultOpenedHandler()
        {
            NavigateToApplications();
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
            Globals.vaultOpen = false;
        }

        private void NavigateToApplications()
        {
            VaultsTab.Visibility = Visibility.Collapsed;
            ApplicationsTab.Visibility = Visibility.Visible;
            MainTabControl.SelectedItem = ApplicationsTab;

            ApplicationsView?.RefreshApplicationListView();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Globals.vaultOpen)
            {
                AddVault addVault = new AddVault();
                addVault.ShowDialog();
            }
            else
            {
                AddApplication addApplication = new AddApplication();
                addApplication.ShowDialog();

                if (Globals.closeAppConfirmation == false)
                {
                    ApplicationsView?.AddApplication();
                }
            }
        }

        private void vaultIconButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToVaults();
        }
    }
}