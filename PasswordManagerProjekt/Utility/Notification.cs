using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwM_Library
{
    public class Notification
    {
        //gridcolor: green = success, orange = warning, red = error
        public static void ShowNotificationInfo(string gridColor, string messageData)
        {
            Globals.gridColor = gridColor;
            Globals.messageData = messageData;
            //TODO
            //PopMessage popMessage = new PopMessage();
            //popMessage.ShowDialog();
        }
    }
}
