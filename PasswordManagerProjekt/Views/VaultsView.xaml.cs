using PwM_UI.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        private ListViewItem VaultList;
        private string VaultPath;
        public VaultsView()
        {
            InitializeComponent();
            LoadVaults();
        }

        public void LoadVaults()
        {
            VaultManager.ListVaults(PwM_Library.Globals.passwordManagerDirectory,
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
            OpenVault();
        }

        private void OpenVault()
        {
            //TODO
        }

    }
}
