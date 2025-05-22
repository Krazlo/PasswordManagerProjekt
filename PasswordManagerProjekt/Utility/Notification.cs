using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PwM_Library;
using PwM_UI.Views;

namespace PwM_UI.Utility
{
    public class Notification
    {
        //gridcolor: green = success, orange = warning, red = error
        public static void ShowNotificationInfo(int gridColor, string messageData)
        {
            Globals.gridColor = gridColor;
            Globals.messageData = messageData;
            PopMessage popMessage = new PopMessage();
            popMessage.ShowDialog();
        }
    }
}
