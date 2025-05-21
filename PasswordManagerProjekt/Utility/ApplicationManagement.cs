using PwM;
using PwM_Library;
using PwMLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PwM_UI.Utility
{
    public class AppManagement
    {
        public static SecureString vaultSecure = null;
        private static string passMask = "\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022";
        private static string passMaskBreach = "\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022       \u24B7";
        
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
                string decryptVault = AesHelper.Decrypt(readVault, PasswordService.SS2S(masterPassword));
                if (decryptVault.Contains("Error decrypting"))
                {
                    Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, "Error DecryptAndPopulateList: Master password is incorrect (or issue with vault)");
                    Globals.masterPasswordCheck = false;
                    return false;
                }

                vaultSecure = PasswordService.S2SS(decryptVault);
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

        public static void DeleteApplicaiton(ListView listView, string vaultName, string application, string accountName, SecureString masterPassword, string vaultPath)
        {
            List<string> listApps = new List<string>();
            bool accountCheck = false;
            string pathToVault  = Path.Combine(Globals.passwordManagerDirectory, $"{vaultName}.x"); ;
            if (masterPassword == null)
            {
                UtilityFunctions.VariablesClear();
                return;
            }
            if (!File.Exists(pathToVault))
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, $"Error DeleteApplicaiton: Vault {vaultName} does not exist!");
                return;
            }
            string readVault = File.ReadAllText(pathToVault);
            string decryptVault = AesHelper.Decrypt(readVault, PasswordService.SS2S(masterPassword));
            if (decryptVault.Contains("Error decrypting"))
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, "Error DeleteApplicaiton: Master password is incorrect (or issue with vault)");
                Globals.masterPasswordCheck = false;
                return;
            }
            if (accountName.Length < 3)
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, "Error DeleteApplicaiton: name length less than 3");
                return;
            }

            if (!decryptVault.Contains(application))
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, $"Error DeleteApplicaiton: application {application} does not exist");
                return;
            }
            using (var reader = new StringReader(decryptVault))
            {
                string line;
                listView.Items.Clear();
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {
                        listApps.Add(line);
                        var outJson = JsonSerializer.Deserialize<Dictionary<string, string>>(line);
                        if (outJson["site/application"] == application && outJson["account"] == accountName)
                        {
                            listApps.Remove(line);
                            accountCheck = true;
                        }
                    }
                }
                string encryptdata = AesHelper.Encrypt(string.Join("\n", listApps), PasswordService.SS2S(masterPassword));
                vaultSecure = PasswordService.S2SS(string.Join("\n", listApps));
                listApps.Clear();
                if (File.Exists(pathToVault))
                {
                    if (accountCheck)
                    {
                        try
                        {
                            File.WriteAllText(pathToVault, encryptdata);
                            Notification.ShowNotificationInfo((int)Globals.MsgLvl.Notification, $"Account {accountName} for {application} was deleted");
                            return;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, $"Error DeleteApplicaiton: Unauthorized.");
                            return;
                        }
                    }
                    Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, $"Error DeleteApplicaiton: Account {accountName} does not exist!");
                    return;
                }
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, $"Error DeleteApplicaiton: Vault {vaultName} does not exist!");
            }
        }

        public static void UpdateAccount(ListView listView, string vaultName, string application, string accountName, string password, SecureString masterPassword, string vaultPath)
        {
            List<string> listApps = new List<string>();
            bool accountCheck = false;
            string pathToVault = pathToVault = Path.Combine(Globals.passwordManagerDirectory, $"{vaultName}.x");
            
            if (!File.Exists(pathToVault))
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, $"Error UpdateAccount: Vault {vaultName} does not exist");
                return;
            }
            if (masterPassword == null)
            {
                UtilityFunctions.VariablesClear();
                return;
            }
            string readVault = File.ReadAllText(pathToVault);
            string decryptVault = AesHelper.Decrypt(readVault, PasswordService.SS2S(masterPassword));
            if (decryptVault.Contains("Error decrypting"))
            {
                Globals.masterPasswordCheck = false;
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, "Error UpdateAccount: Master password incorrect (or issue with vault)");
                return;
            }
            if (accountName.Length < 3)
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, "Error UpdateAccount: length less than 3");
                return;
            }
            if (!decryptVault.Contains(application))
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, $"Error UpdateAccount: application {application} does not exist!");
                return;
            }
            using (var reader = new StringReader(decryptVault))
            {
                string line;
                listView.Items.Clear();
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {
                        var outJson = JsonSerializer.Deserialize<Dictionary<string, string>>(line);
                        if (outJson["site/application"] == application && outJson["account"] == accountName)
                        {
                            var keyValues = new Dictionary<string, object>
                            {
                                 { "site/application", application },
                                 { "account", accountName },
                                 { "password", password },
                            };
                            accountCheck = true;
                            listApps.Add(JsonSerializer.Serialize(keyValues));
                        }
                        else
                        {
                            listApps.Add(line);
                        }
                    }
                }
                string encryptdata = AesHelper.Encrypt(string.Join("\n", listApps), PasswordService.SS2S(masterPassword));
                vaultSecure = PasswordService.S2SS(string.Join("\n", listApps));
                listApps.Clear();
                if (File.Exists(pathToVault))
                {
                    if (accountCheck)
                    {
                        try
                        {
                            File.WriteAllText(pathToVault, encryptdata);
                            Notification.ShowNotificationInfo((int)Globals.MsgLvl.Notification, $"Success: Password for account {accountName} for {application} application was updated!");
                            return;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, $"Error UpdateAccount: Unauthorized");
                            return;
                        }
                    }
                    Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, $"Warning: Account {accountName} does not exist!");
                    return;
                }
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Error, $"Error: vault {vaultName} does not exist!");
            }
            ListViewSettings.ListViewSortSetting(listView, "site/application", false);
        }

        
        public static void ShowPassword(ListView listView)
        {
            var isPasswordBreachMask = false;
            ListView tempListView = new ListView();
            if (listView.SelectedItem == null)
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, "Warning: You must first select an application");
                return;
            }

            string selectedItem = listView.SelectedItem.ToString();
            if (selectedItem.Contains("\u24B7"))
                selectedItem = selectedItem.Replace($", Password = {passMaskBreach} " + "}", string.Empty);
            else
                selectedItem = selectedItem.Replace($", Password = {passMask} " + "}", string.Empty);
            selectedItem = selectedItem.Replace("{ Application = ", string.Empty);
            selectedItem = selectedItem.Replace(", Account = ", "|");
            var parsedData = selectedItem.Split('|');
            if (Globals.listItems.Count == 1)
                isPasswordBreachMask = string.Join("", Globals.listItems).Contains("\u24B7");
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
                            if (isBreachMask)
                                tempListView.Items.Add(new { Application = outJson["site/application"], Account = outJson["account"], Password = passMaskBreach });
                            else
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
                        if (isPasswordBreachMask)
                            tempListView.Items.Add(new { Application = outJson["site/application"], Account = outJson["account"], Password = passMaskBreach });
                        else
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

        public static string CopyPassToClipBoard(ListView listView)
        {
            string outPass = string.Empty;
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
                            Globals.accountPassword = outJson["password"];
                            Notification.ShowNotificationInfo((int)Globals.MsgLvl.Notification, $"Password for {account} is copied to clipboard!");
                        }
                    }
                }
            }
            return outPass;
        }

        public static void DeleteSelectedItem(ListView listView, string vaultName, string vaultPath, ListView vaultList)
        {
            var pathToVault = Path.Combine(vaultPath, $"{vaultName}.x");
            string application = GetApplicationFromListView(listView);
            if (application.Length > 0)
            {
                string account = GetAccountFromListView(listView);
                if (account.Length > 0 && application.Length > 0)
                {
                    Globals.accountName = account;
                    Globals.applicationName = application;
                    DelApplications delApplications = new DelApplications();
                    delApplications.ShowDialog();
                    if (Globals.deleteConfirmation)
                    {
                        if (!Globals.masterPasswordCheck)
                        {
                            var masterPassword = UtilityFunctions.LoadMasterPassword(vaultName);
                            DeleteApplicaiton(listView, vaultName, application, account, masterPassword, vaultPath);
                            UtilityFunctions.VariablesClear();
                            return;
                        }
                        DeleteApplicaiton(listView, vaultName, application, account, Globals.masterPassword, vaultPath);
                        UtilityFunctions.VariablesClear();
                    }
                }
            }
            else
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, "Warning DeleteSelectedItem: You must select an application to delete!");
            }
            ListViewSettings.ListViewSortSetting(listView, "site/application", false);
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

        public static void UpdateSelectedItemPassword(ListView listView, string vaultName, string vaultPath)
        {
            var pathToVault = Path.Combine(vaultPath, $"{vaultName}.x");
            string application = GetApplicationFromListView(listView);
            string account = GetAccountFromListView(listView);
            if (account.Length > 0 && application.Length > 0)
            {
                Globals.accountName = account;
                Globals.applicationName = application;
                //TODO
                //UpdateApplication updateApplication = new UpdateApplication();
                //updateApplication.ShowDialog();
                string newPassword = Globals.newAccountPassword;
                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (!Globals.masterPasswordCheck)
                    {
                        var masterPassword = UtilityFunctions.LoadMasterPassword(vaultName);
                        UpdateAccount(listView, vaultName, application, account, newPassword, masterPassword, vaultPath);
                        UtilityFunctions.VariablesClear();
                        return;
                    }
                    UpdateAccount(listView, vaultName, application, account, newPassword, Globals.masterPassword, vaultPath);
                    UtilityFunctions.VariablesClear();
                }
            }
            ListViewSettings.ListViewSortSetting(listView, "site/application", false);
        }

        public static void AddAppsToTempList(ListView listView)
        {
            Globals.listItems.Clear();
            for (int i = 0; i <= listView.Items.Count - 1; i++)
                Globals.listItems.Add(listView.Items[i].ToString());
        }
    }
}
