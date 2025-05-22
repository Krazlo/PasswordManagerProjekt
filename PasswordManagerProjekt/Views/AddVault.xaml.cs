using Microsoft.Win32;
using PwM_Library;
using PwM_UI.Utility;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PwM_UI.Views
{
    public partial class AddVault : Window
    {
        public AddVault()
        {
            InitializeComponent();
            VaultNameTextBox.Focus();

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged; // Exit vault on suspend.
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch); // Exit vault on lock screen.
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Suspend:
                    PwM_Library.Globals.closeAppConfirmation = true;
                    this.Close();
                    break;
            }
        }


        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                PwM_Library.Globals.closeAppConfirmation = true;
                this.Close();
            }
        }

        private void addVPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            createBtn.IsEnabled = (ConfirmPasswordBox.Password == MasterPasswordBox.Password && MasterPasswordBox.Password.Length >= 12);
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string password = MasterPasswordBox.Password;

            //Validation done in CreateVault
            VaultManager.CreateVault(
                VaultNameTextBox.Text, 
                MasterPasswordBox.Password, 
                ConfirmPasswordBox.Password, 
                Globals.passwordManagerDirectory);

            if (Globals.vaultChecks)
            {
               TextPassBoxChanges.ClearPBoxesInput(MasterPasswordBox, ConfirmPasswordBox);
                Globals.vaultChecks = false;
            }
            else
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    if (mainWindow.VaultsView is VaultsView vaultsView)
                    {
                        vaultsView.RefreshVaults();
                    }
                }
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string password = PasswordService.PasswordGenerator.GeneratePassword();

            MasterPasswordBox.Password = password;
            ConfirmPasswordBox.Password = password;
            
        }
    }
}