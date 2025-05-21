using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PwM_Library
{
    //For Global Variables
    public static class Globals
    {
        public static List<string> listItems = new List<string>();
        public static int vaultsCount { get; set; }
        public static bool vaultOpen = false;
        public static bool deleteConfirmation = false;
        public static bool createConfirmation = false;
        public static bool importConfirmation = false;
        public static bool updatePwdConfirmation = false;
        public static bool closeAppConfirmation = false;
        public static SecureString masterPassword { get; set; }
        public static SecureString newMasterPassword { get; set; }
        public static bool masterPasswordCheck = true;
        public static string applicationName { get; set; }
        public static string accountPassword { get; set; }
        public static string accountName { get; set; }
        public static string newAccountPassword { get; set; }
        public static string vaultName { get; set; }
        public static int gridColor { get; set; }
        public static string messageData { get; set; }
        public static bool vaultChecks = false;
        private static string s_rootPath = Path.GetPathRoot(Environment.SystemDirectory);
        private static readonly string s_accountName = Environment.UserName;
        public static readonly string passwordManagerDirectory = $"{s_rootPath}Users\\{s_accountName}\\AppData\\Local\\PwM\\";
        public static readonly string registryPath = "SOFTWARE\\PwM";

        public enum MsgLvl
        {
            Notification = 1,
            Error = 2,
            Warning = 3
        }
    }
}
