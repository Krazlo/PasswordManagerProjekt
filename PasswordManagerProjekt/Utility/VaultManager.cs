using PwM_Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace PwM_UI.Utility
{
    public static class VaultManager
    {
        public static void CreateVault(string vaultName, string password, string confirmPassword, string vaultDirectory)
        {
            try
            {
                string pathToVault = Path.Combine(vaultDirectory, $"{vaultName}.x");
                if (File.Exists(pathToVault))
                {
                    Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, $"Warning CreateVault: Vault {vaultName} already exist!");
                    Globals.vaultChecks = true;
                    return;
                }

                if (vaultName.Length < 3)
                {
                    Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, "Warning CreateVault: Vault name must be at least 3 characters long.");
                    Globals.vaultChecks = true;
                    return;
                }

                if (!PasswordService.ValidatePassword(confirmPassword))
                {
                    Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, "Warning CreateVault: Password must be at least 12 characters and include at least 1 upper case, 1 lower case, 1 digit, 1 special character and no white space!");
                    Globals.vaultChecks = true;
                    return;
                }

                string sealVault = AesHelper.Encrypt(string.Empty, confirmPassword);
                File.WriteAllText(pathToVault, sealVault);
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Notification, $"Vault {vaultName} was created!");
            }
            catch (Exception e)
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, e.Message);
            }
        }

        public static string GetVaultNameFromListView(ListView listView)
        {
            string application = string.Empty;
            if (listView.SelectedItem == null)
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, "Warning GetVaultNameFromListView: You must select a vault to delete!");
                return application;
            }
            string selectedItem = listView.SelectedItem.ToString();
            application = selectedItem.SplitByText(", ", 0).Replace("{ Name = ", string.Empty);
            return application;
        }

        public static void ListVaults(string vaultsDirectory, ListView listView, bool enableShare)
        {
            Globals.vaultsCount = 0;
            listView.Items.Clear();

            if (enableShare)
                vaultsDirectory = Globals.passwordManagerDirectory;

            if (!Directory.Exists(vaultsDirectory))
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, "Vaults directory does not exist");
                return;
            }

            var getFiles = new DirectoryInfo(vaultsDirectory).GetFiles();
            foreach (var file in getFiles)
            {
                Globals.vaultsCount++;
                if (file.Name.EndsWith(".x"))
                {
                    listView.Items.Add(new { Name = file.Name.Substring(0, file.Name.Length - 2), CreateDate = file.CreationTime });
                }
            }
        }

        public static string GetVaultPathFromList(ListView listView)
        {
            string vaultPath = string.Empty;
            if (listView.SelectedItem != null)
            {
                dynamic item = listView.SelectedItem;
                vaultPath = item.Name;
                vaultPath = vaultPath.Replace(" }", "");
            }
            return vaultPath;
        }
    }
}
