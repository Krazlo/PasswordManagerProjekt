using System.Windows;
using PwM_Library;
using System.Windows.Controls;
using Microsoft.Win32;

namespace PwM_UI.Views
{
    public partial class AddApplication : Window
    {
        public AddApplication()
        {
            InitializeComponent();
            AppNameTextBox.Focus();

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged; // Exit vault on suspend.
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch); // Exit vault on lock screen.
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.applicationName = AppNameTextBox.Text;
            Globals.accountName = AccountNameTextBox.Text;
            Globals.accountPassword = AccountPasswordBox.Password;
            Globals.closeAppConfirmation = false;
            Utility.TextPassBoxChanges.ClearTextPassBox(AppNameTextBox, AccountNameTextBox, AccountPasswordBox);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string password = PasswordService.PasswordGenerator.GeneratePassword();

            AccountPasswordBox.Password = password;
        }

        private void ToggleVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (AccountPasswordBox.Visibility == Visibility.Visible)
            {
                PasswordTextBox.Text = AccountPasswordBox.Password;
                PasswordTextBox.Visibility = Visibility.Visible;
                AccountPasswordBox.Visibility = Visibility.Collapsed;
                ToggleVisibilityButton.Content = "🙈";
            }
            else
            {
                AccountPasswordBox.Password = PasswordTextBox.Text;
                AccountPasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                ToggleVisibilityButton.Content = "👁️";
            }
        }
    }
}