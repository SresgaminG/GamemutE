using System;
using System.Windows.Forms;

namespace SresgaminG.Arma
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

        public static string ApplicationVersion
        {
            get
            {
                string version = Application.ProductVersion;

                string[] array = version.Split('.');

                if (Convert.ToInt32(array[2]) > 0)
                    return string.Format("{0}.{1}.{2}", array[0], array[1], array[2]);

                return string.Format("{0}.{1}", array[0], array[1]);
            }
        }
    }
}
