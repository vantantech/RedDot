using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class TapTipKeyboard
    {

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string sClassName, string sAppName);

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
        public static void ShowKeyboard()
        {
            string _virtualKeyboardPath;
            _virtualKeyboardPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), @"Microsoft Shared\ink\TabTip.exe");
            Process.Start(_virtualKeyboardPath);
        }

        public static void HideKeyboard()
        {
            var nullIntPtr = new IntPtr(0);
            const uint wmSyscommand = 0x0112;
            var scClose = new IntPtr(0xF060);

            var keyboardWnd = FindWindow("IPTip_Main_Window", null);
            if (keyboardWnd != nullIntPtr)
            {
                SendMessage(keyboardWnd, wmSyscommand, scClose, nullIntPtr);
            }
        }
    }
}
