using Microsoft.Win32;
using PwM_Library;
using System.Runtime.Versioning;
using System.Windows;


namespace PwM
{
    [SupportedOSPlatform("Windows")]
    /// <summary>
    /// Interaction logic for DelApplications.xaml
    /// </summary>
    public partial class DelApplications : Window
    {
        public DelApplications()
        {
            InitializeComponent();

            string application = Globals.applicationName;
            string account = Globals.accountName;
            notificationLBL.Text = $"Do you want tot delete {account} account for {application} application?";
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged; // Exit vault on suspend.
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch); // Exit vault on lock screen.
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Suspend:
                    Globals.applicationName = "";
                    Globals.accountName = "";
                    Globals.deleteConfirmation = false;
                    this.Close();
                    break;
            }
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                Globals.applicationName = "";
                Globals.accountName = "";
                Globals.deleteConfirmation = false;
                this.Close();
            }
        }

        /// <summary>
        /// Close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void confirmBTN_Click(object sender, RoutedEventArgs e)
        {
            Globals.deleteConfirmation = true;
            this.Close();
        }

        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            Globals.applicationName = "";
            Globals.accountName = "";
            Globals.deleteConfirmation = false;
            this.Close();
        }
    }
}
