using PwM_Library;
using PwM_UI.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PwM_UI.Utility
{
    internal static class UtilityFunctions
    {
        internal static string SplitByText(this string input, string parameter, int index)
        {
            string[] output = input.Split(new string[] { parameter }, StringSplitOptions.None);
            return output[index];
        }

        public static void ClearClipboard(string accPassword)
        {
            if (Mkb.ClipBoardManager.GetText() == accPassword)
            {
                Mkb.ClipBoardManager.Clear();
            }
        }

        public static void VariablesClear()
        {
            Globals.applicationName = "";
            Globals.accountName = "";
            Globals.newAccountPassword = "";
            Globals.closeAppConfirmation = false;
        }

        public static SecureString LoadMasterPassword(string vaultName)
        {
            SecureString password = null;
            Globals.vaultName = vaultName;
            MasterPassword masterPassword = new MasterPassword(vaultName);
            masterPassword.ShowDialog();
            password = masterPassword.masterPassword;
            //masterPassword.masterPassword.Clear();
            //masterPassword.masterPassword = null;
            return password;
        }
    }
}
