using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class TouchMessage
    {

        public static void Show(string message)
        {

            new TouchMessageBox(message).ShowDialog();


        }


        public static void ShowError(string message)
        {
            new AlertView(message).ShowDialog();


        }
    }
}
