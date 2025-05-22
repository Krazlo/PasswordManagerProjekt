using PwM_Library;
using System.IO;
using System.Windows;
using PwM_UI.Utility;
using System.Security;
using Microsoft.Win32;


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
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged; // Exit vault on suspend.
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch); // Exit vault on lock screen.

        }
        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Suspend:
                    this.Close();
                    break;
            }
        }
        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
                this.Close();
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
            Globals.masterPassword = MasterPasswordBox.SecurePassword;
            masterPassword = MasterPasswordBox.SecurePassword;
            this.Close();
        }

        private void ToggleVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (MasterPasswordBox.Visibility == Visibility.Visible)
            {
                PasswordTextBox.Text = MasterPasswordBox.Password;
                PasswordTextBox.Visibility = Visibility.Visible;
                MasterPasswordBox.Visibility = Visibility.Collapsed;
                ToggleVisibilityButton.Content = "🙈";
            }
            else
            {
                MasterPasswordBox.Password = PasswordTextBox.Text;
                MasterPasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                ToggleVisibilityButton.Content = "👁️";
            }
        }
    }
}
