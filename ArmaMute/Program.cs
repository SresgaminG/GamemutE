using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
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

            if (UserHasCurrentVersion())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainWindow());
            }
        }

        private static bool UserHasCurrentVersion()
        {
            LogHelper.Info(null, "Checking for new version");

            WebClient client = new WebClient();
            string webPage = client.DownloadString("https://github.com/SresgaminG/ArmamutE/releases");

            string[] pageArray = webPage.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            string latestVersionString = pageArray.Where(pa => pa.Trim().StartsWith("<span class=\"css-truncate-target\">")).ToList()[0];
            var latestVersionValue = latestVersionString.Substring(latestVersionString.IndexOf('>') + 1, (latestVersionString.LastIndexOf('<') - latestVersionString.IndexOf('>') - 1));

            if (latestVersionValue != Program.ApplicationVersion)
            {
                LogHelper.Info(null, "There is a later version available (Version {0})", latestVersionValue);

                switch (MessageBox.Show(string.Format("There is a new version of ArmamutE available (Version {0}). Do you want to download it now?", latestVersionValue),
                    string.Format("ArmamutE Version {0}", Program.ApplicationVersion), MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
                {
                    case DialogResult.Yes:

                        LogHelper.Debug(null, "User selected download");

                        Process.Start(string.Format("https://github.com/SresgaminG/ArmamutE/releases/download/{0}/ArmamutE.msi", latestVersionValue));

                        return false;
                    case DialogResult.No:
                        return true;
                }
            }

            LogHelper.Debug(null, "Latest version already installed");

            return true;
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
