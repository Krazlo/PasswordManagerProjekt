using Microsoft.Win32;
using PwM_Library;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Media;

namespace PwM
{
    [SupportedOSPlatform("Windows")]
    /// <summary>
    /// Interaction logic for PopMessage.xaml
    /// </summary>
    public partial class PopMessage : Window
    {
        public PopMessage()
        {
            InitializeComponent();
            SetUI(Globals.gridColor, Globals.messageData);
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

        private void confirmBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SetUI(int gridColor, string messageData)
        {
            switch (gridColor)
            {
                case 1:
                    popGrid.Background = Brushes.Green;
                    titleTxt.Text = "Notification";
                    notificationLBL.Text = messageData;
                    break;

                case 2:
                    popGrid.Background = Brushes.Red;
                    titleTxt.Text = "ERROR";
                    notificationLBL.Text = messageData;
                    break;

                case 3:
                    popGrid.Background = Brushes.DarkOrange;
                    titleTxt.Text = "WARNING";
                    notificationLBL.Text = messageData;
                    break;
            }
        }
    }
}