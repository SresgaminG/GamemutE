using System;
using System.Windows.Forms;

namespace SresgaminG.Arma3
{
    static class Program
    {
        public static SingleInstanceHelper SingleInstanceHelper { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SingleInstanceHelper = new SingleInstanceHelper();

            //If not first instance
            if (!SingleInstanceHelper.Start(SingleInstanceHelper.SingleInstanceScope.CurrentUser))
            {
                //Show first instance and return
                SingleInstanceHelper.ShowFirstInstance(); return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}
