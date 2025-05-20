using PwM_Library;
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
            Globals.deleteConfirmation = false;
        }

        public static SecureString LoadMasterPassword(string vaultName)
        {
            SecureString password = null;
            Globals.vaultName = vaultName;
            //TODO
            /*MasterPassword masterPassword = new MasterPassword();
            masterPassword.ShowDialog();
            password = masterPassword.masterPassword;
            masterPassword.masterPasswordPWD.Clear();
            masterPassword.masterPassword = null;*/
            return password;
        }
    }
}
