using PwM_Library;
using PwM_UI.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PwM_UI.Views
{
    /// <summary>
    /// Interaction logic for VaultsView.xaml
    /// </summary>
    public partial class VaultsView : UserControl
    {
        public Action OnVaultOpened;

        public VaultsView()
        {
            InitializeComponent();
            LoadVaults();
        }

        public void LoadVaults()
        {
            VaultManager.ListVaults(Globals.passwordManagerDirectory,
                                  VaultsListView,
                                  false);
        }

        public void RefreshVaults()
        {
            VaultsListView.Items.Clear();
            LoadVaults();
        }

        private void VaultsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VaultsListView.SelectedItem != null)
            {
                dynamic selected = VaultsListView.SelectedItem;
                if (!OpenVault()) 
                {
                    return;
                }

                OnVaultOpened.Invoke();
            }

        }

        private bool OpenVault()
        {
            string vaultPath = VaultManager.GetVaultPathFromList(VaultsListView);
            string item = VaultsListView.SelectedItem.ToString();
            string vaultName = item.Split(',')[0].Replace("{ Name = ", "");
            var vaultFullPath = $"{vaultPath}\\{vaultName}.x";
            var masterPassword = UtilityFunctions.LoadMasterPassword(vaultName);

            Globals.masterPassword = masterPassword;
            if (masterPassword != null)
            {
                Globals.vaultName = vaultName;
                Globals.vaultOpen = true;
                Globals.vaultPath = vaultPath;

                return true;
            }
            Globals.vaultName = "";
            Globals.vaultOpen = false;
            Globals.vaultPath = "";
            return false;
        }
    }
}
