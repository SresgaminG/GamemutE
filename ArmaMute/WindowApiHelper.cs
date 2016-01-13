using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SresgaminG.Arma3
{
    public static class WindowApiHelper
    {

        #region Constants

        public const int HWND_BROADCAST = 0xffff;
        public const int SW_SHOWNORMAL = 1;
        public const int WM_SETREDRAW = 11;

        #endregion

        #region DLL Imports

        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        [DllImportAttribute("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImportAttribute("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion

        #region Methods

        public static int RegisterWindowMessage(String format, params Object[] args)
        {
            return RegisterWindowMessage(String.Format(format, args));
        }

        public static void ShowToFront(IntPtr window)
        {
            //Show window
            ShowWindow(window, SW_SHOWNORMAL);

            //Bring to front
            SetForegroundWindow(window);
        }

        public static void SuspendDrawing(this Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(this Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, true, 0);
            control.Refresh();
        }


        #endregion
    }
}
