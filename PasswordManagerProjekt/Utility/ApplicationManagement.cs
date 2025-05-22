using PwM_Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PwM_UI.Utility
{
    public class AppManagement
    {
        public static SecureString vaultSecure = null;
        private static string passMask = "\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022";

        // Decrypts vault and populates with applications
        public static bool DecryptAndPopulateList(ListView listView, string vaultName, SecureString masterPassword, string vaultPath)
        {
            try
            {
                listView.Items.Clear();
                string pathToVault = Path.Combine(Globals.passwordManagerDirectory, $"{vaultName}.x");

                if (!File.Exists(pathToVault))
                {
                    Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, $"Error DecryptAndPopulateList: Vault {vaultName} does not exist!");
                    return false;
                }

                if (masterPassword == null)
                {
                    Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, "Error DecryptAndPopulateList: Missing master password");
                    return false;
                }
                string readVault = File.ReadAllText(pathToVault);
                string spw = PasswordService.SS2S(masterPassword);
                string decryptVault = AesHelper.Decrypt(readVault, spw);
                if (decryptVault.Contains("Error decrypting"))
                {
                    Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, "Error DecryptAndPopulateList: Master password is incorrect (or issue with vault)");
                    Globals.masterPasswordCheck = false;
                    return false;
                }

                vaultSecure = PasswordService.S2SS(decryptVault);
                using (var reader = new StringReader(decryptVault))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (!String.IsNullOrEmpty(line))
                        {
                            var outJson = JsonSerializer.Deserialize<Dictionary<string, string>>(line);
                            //outJson["password"]
                            if (outJson != null)
                                listView.Items.Add(new { Application = outJson["site/application"], Account = outJson["account"], Password = passMask });

                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, e.Message);
                return false;
            }
        }

        public static void AddApplication(ListView listView, string vaultName, string application, string accountName, string accountPassword, SecureString masterPassword, string vaultPath)
        {
            string pathToVault = Path.Combine(Globals.passwordManagerDirectory, $"{vaultName}.x");

            if (!File.Exists(pathToVault))
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, $"Error AddApplication: Vault {vaultName} does not exist!");
                return;
            }

            if (masterPassword == null)
            {
                UtilityFunctions.VariablesClear();
                return;
            }
            foreach (var item in listView.Items)
            {
                string app = item.ToString().SplitByText(", ", 0).Replace("{ Application = ", string.Empty);
                string acc = item.ToString().SplitByText(", ", 1).Replace("Account = ", string.Empty);
                if (app == (application) && acc == (accountName))
                {
                    Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, $"Error AddApplication: Application {application} already contains {accountName}");
                    return;
                }
            }
            string readVault = File.ReadAllText(pathToVault);
            string decryptVault = AesHelper.Decrypt(readVault, PasswordService.SS2S(masterPassword));
            if (decryptVault.Contains("Error decrypting"))
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, "Error AddApplication: Master password is incorrect (or issue with vault)\"");
                Globals.masterPasswordCheck = false;
                return;
            }
            if (accountName.Length < 3)
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, "Error AddApplication: name length less than 3");
                return;
            }
            var keyValues = new Dictionary<string, object>
                {
                    { "site/application", application },
                    { "account", accountName },
                    { "password", accountPassword },
                };
            string encryptdata = AesHelper.Encrypt(decryptVault + "\n" + JsonSerializer.Serialize(keyValues), PasswordService.SS2S(masterPassword));
            vaultSecure = PasswordService.S2SS(decryptVault + "\n" + JsonSerializer.Serialize(keyValues));
            if (File.Exists(pathToVault))
            {
                try
                {
                    File.WriteAllText(pathToVault, encryptdata);
                    Notification.ShowNotificationInfo((int)Globals.MsgLvl.Notification, $"{application} has been encrypted and added to the vault");
                    listView.Items.Add(new { Application = application, Account = accountName, Password = passMask });
                    return;
                }
                catch (UnauthorizedAccessException)
                {
                    Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, $"Error AddApplication: Access denied.");
                    return;
                }
            }
            Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, $"Error AddApplication: Vault {vaultName} does not exist!");
            ListViewSettings.ListViewSortSetting(listView, "site/application", false);
        }

        public static void ShowPassword(ListView listView)
        {
            ListView tempListView = new ListView();
            if (listView.SelectedItem == null)
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, "Warning: You must first select an application");
                return;
            }

            string selectedItem = listView.SelectedItem.ToString();
            selectedItem = selectedItem.Replace($", Password = {passMask} " + "}", string.Empty);
            selectedItem = selectedItem.Replace("{ Application = ", string.Empty);
            selectedItem = selectedItem.Replace(", Account = ", "|");
            var parsedData = selectedItem.Split('|');
            var vault = PasswordService.SS2S(vaultSecure);
            if (CountLines(vault) >= 2)
            {
                var vaultToLines = PasswordService.SS2S(vaultSecure).Split(new[] { '\r', '\n' });
                foreach (var line in vaultToLines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        var outJson = JsonSerializer.Deserialize<Dictionary<string, string>>(line);
                        if (outJson["site/application"] == parsedData[0] && outJson["account"] == parsedData[1])
                            tempListView.Items.Add(new { Application = outJson["site/application"], Account = outJson["account"], Password = outJson["password"] });
                        else
                        {
                            var isBreachMask = false;
                            foreach (var item in Globals.listItems)
                            {
                                var jsonSplit = item.Split(',');
                                if (jsonSplit[0].Contains(outJson["site/application"]) && jsonSplit[1].Contains(outJson["account"]) && jsonSplit[2].Contains("\u24B7"))
                                {
                                    isBreachMask = true;
                                    break;
                                }
                            }
                            tempListView.Items.Add(new { Application = outJson["site/application"], Account = outJson["account"], Password = passMask });
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(vault))
                {
                    var outJson = JsonSerializer.Deserialize<Dictionary<string, string>>(vault);
                    if (outJson["site/application"] == parsedData[0] && outJson["account"] == parsedData[1])
                        tempListView.Items.Add(new { Application = outJson["site/application"], Account = outJson["account"], Password = outJson["password"] });
                    else
                    {
                        tempListView.Items.Add(new { Application = outJson["site/application"], Account = outJson["account"], Password = passMask });
                    }
                }
            }
            listView.Items.Clear();
            foreach (var item in tempListView.Items)
            {
                listView.Items.Add(item);
            }
            tempListView.Items.Clear();
            ListViewSettings.ListViewSortSetting(listView, "site/application", false);
        }

        private static int CountLines(string vaultData)
        {
            var lines = vaultData.Split(new[] { '\r', '\n' });
            int count = lines.Where(x => !string.IsNullOrEmpty(x)).Count();
            return count;
        }

        public static void CopyPassToClipBoard(ListView listView)
        {
            string outPass = string.Empty;
            if (listView.SelectedItem == null)
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, "Warning: You must first select an application");
            }

            string application = GetApplicationFromListView(listView);
            string account = GetAccountFromListView(listView);
            if (account.Length > 0 && application.Length > 0)
            {
                var lines = PasswordService.SS2S(vaultSecure).Split(new[] { '\r', '\n' });
                foreach (var line in lines)
                {
                    if (line.Length > 0)
                    {
                        var outJson = JsonSerializer.Deserialize<Dictionary<string, string>>(line);
                        if (outJson["site/application"] == application && outJson["account"] == account)
                        {
                            outPass = outJson["password"];
                            Clipboard.SetText(outPass);
                        }
                    }
                }
            }
        }

        private static string GetAccountFromListView(ListView listView)
        {
            string account = string.Empty;
            if (listView.SelectedItem == null)
            {
                return account;
            }
            string selectedItem = listView.SelectedItem.ToString();
            account = selectedItem.SplitByText(", ", 1).Replace("Account = ", string.Empty);
            return account;
        }

        private static string GetApplicationFromListView(ListView listView)
        {
            string application = string.Empty;
            if (listView.SelectedItem == null)
            {
                return application;
            }
            string selectedItem = listView.SelectedItem.ToString();
            application = selectedItem.SplitByText(", ", 0).Replace("{ Application = ", string.Empty);
            return application;
        }


        public static void AddAppsToTempList(ListView listView)
        {
            Globals.listItems.Clear();
            for (int i = 0; i <= listView.Items.Count - 1; i++)
                Globals.listItems.Add(listView.Items[i].ToString());
        }
    }
}
