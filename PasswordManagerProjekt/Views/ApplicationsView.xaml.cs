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
    /// Interaction logic for ApplicationsView.xaml
    /// </summary>
    /// 

    public partial class ApplicationsView : UserControl
    {
        public Action RefreshApplicationView;
        public ApplicationsView()
        {
            InitializeComponent();
        }

        public void RefreshApplicationListView()
        {
            if (AppManagement.DecryptAndPopulateList(ApplicationsList, Globals.vaultName, Globals.masterPassword, Globals.vaultPath))
            {
                Globals.vaultOpen = true;
                Sort("Application", ApplicationsList, ListSortDirection.Ascending);
                AppManagement.AddAppsToTempList(ApplicationsList);
            }
        }

        public void AddApplication()
        {
            if (Globals.checkMasterPassword)
            {
                var masterPassword = UtilityFunctions.LoadMasterPassword(Globals.vaultName);
                AppManagement.AddApplication(ApplicationsList, Globals.vaultName, Globals.applicationName, Globals.accountName, Globals.accountPassword, masterPassword, Globals.vaultPath);
                AppManagement.AddAppsToTempList(ApplicationsList);
                UtilityFunctions.VariablesClear();
                return;
            }
            AppManagement.AddApplication(ApplicationsList, Globals.vaultName, Globals.applicationName, Globals.accountName, Globals.accountPassword, Globals.masterPassword, Globals.vaultPath);
            AppManagement.AddAppsToTempList(ApplicationsList);
            UtilityFunctions.VariablesClear();
        }

        private void Sort(string sortBy, ListView listView, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(listView.Items);
            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        private void ToggleVisibility_Click(object sender, RoutedEventArgs e)
        {
            AppManagement.ShowPassword(ApplicationsList);
        }

        private void CopyClipboard_Click(object sender, RoutedEventArgs e)
        {
            AppManagement.CopyPassToClipBoard(ApplicationsList);
        }
    }

}
