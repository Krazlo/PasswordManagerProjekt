using PwM_Library;
using System.IO;
using System.Windows;
using PwM_UI.Utility;
using System.Security;


namespace PwM_UI.Views
{
    /// <summary>
    /// Interaction logic for MasterPassword.xaml
    /// </summary>
    public partial class MasterPassword : Window
    {
        public SecureString masterPassword;
        public string VaultName { get; }

        public MasterPassword(string vaultName)
        {
            InitializeComponent();
            VaultName = vaultName;
            DataContext = this;
            MasterPasswordBox.Focus();
        }

        private void UnlockButton_Click(object sender, RoutedEventArgs e)
        {
            if (!PasswordService.ValidatePassword(MasterPasswordBox.Password))
            {
                Notification.ShowNotificationInfo((int)Globals.MsgLvl.Warning, "Warning: Password must be at least 12 characters, and must include at least one upper case letter, one lower case letter, one numeric digit, one special character and no space!");
                MasterPasswordBox.Clear();
                return;
            }
            Globals.masterPasswordCheck = true;
            masterPassword = MasterPasswordBox.SecurePassword;
            this.Close();
        }
    }
}
