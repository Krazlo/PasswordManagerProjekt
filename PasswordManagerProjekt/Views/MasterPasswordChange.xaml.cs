using Microsoft.Win32;
using PwM_Library;
using PwM_UI.Utility;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace PwM_UI.Views
{
    /// <summary>
    /// Interaction logic for MasterPasswordChange.xaml
    /// </summary>
    public partial class MasterPasswordChange : Window
    {
        public MasterPasswordChange()
        {
            InitializeComponent();
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged; // Exit vault on suspend.
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch); // Exit vault on lock screen.
            vaultNameTB.Text = Globals.vaultName;
            Globals.closeAppConfirmation = false;
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Suspend:
                    Globals.closeAppConfirmation = true;
                    this.Close();
                    break;
            }
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                Globals.closeAppConfirmation = true;
                this.Close();
            }
        }
        private void addVPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            createBTN.IsEnabled = (OldMasterPassword.Password.Length >= 12 && NewMasterPassword.Password == ConfirmNewMasterPassword.Password && ConfirmNewMasterPassword.Password.Length >= 12);
        }

        private void ShowVaultPassword(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                BoxHelper.ShowPassword(OldMasterPassword, OldMasterPasswordTXT);
                BoxHelper.ShowPassword(NewMasterPassword, NewMasterPassTXT);
                BoxHelper.ShowPassword(ConfirmNewMasterPassword, ConfirmNewMasterPassTXT);
            }
            else if (e.ButtonState == MouseButtonState.Released)
            {
                BoxHelper.HidePassword(OldMasterPassword, OldMasterPasswordTXT);
                BoxHelper.HidePassword(NewMasterPassword, NewMasterPassTXT);
                BoxHelper.HidePassword(ConfirmNewMasterPassword, ConfirmNewMasterPassTXT);
            }
        }

        private void saveBTN_Click(object sender, RoutedEventArgs e)
        {
            Globals.masterPassword = OldMasterPassword.SecurePassword;
            Globals.newMasterPassword = NewMasterPassword.SecurePassword;
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void confirmVPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            createBTN.IsEnabled = (OldMasterPassword.Password.Length >= 12 && NewMasterPassword.Password == ConfirmNewMasterPassword.Password && ConfirmNewMasterPassword.Password.Length >= 12);
        }

        private void closeLBL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Globals.closeAppConfirmation = true;
            this.Close();
        }

        private void ShowNewVaultPassword_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            BoxHelper.HidePassword(OldMasterPassword, OldMasterPasswordTXT);
            BoxHelper.HidePassword(NewMasterPassword, NewMasterPassTXT);
            BoxHelper.HidePassword(ConfirmNewMasterPassword, ConfirmNewMasterPassTXT);
        }

        private void OldMasterPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            createBTN.IsEnabled = (OldMasterPassword.Password.Length >= 12 && NewMasterPassword.Password == ConfirmNewMasterPassword.Password && ConfirmNewMasterPassword.Password.Length >= 12);
        }
    }
}
