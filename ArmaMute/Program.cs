using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using SresgaminG.GamemutE.Helpers;

namespace SresgaminG.GamemutE
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

            try
            {
                WebClient client = new WebClient();
                string webPage = client.DownloadString("https://github.com/SresgaminG/ArmamutE/releases");

                string[] pageArray = webPage.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                string latestVersionString = pageArray.Where(pa => pa.Trim().StartsWith("<span class=\"css-truncate-target\">")).ToList().First();
                var latestVersionValue = latestVersionString.Substring(latestVersionString.IndexOf('>') + 1, (latestVersionString.LastIndexOf('<') - latestVersionString.IndexOf('>') - 1));

                if (Convert.ToDouble(ApplicationVersion) < Convert.ToDouble(latestVersionValue))
                {
                    LogHelper.Info(null, $"There is a new version available (Version {latestVersionValue})");

                    switch (MessageBox.Show($"There is a new version of GamemutE available (Version {latestVersionValue}). Do you want to download it now?",
                        $"GamemutE {ApplicationVersion}", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
                    {
                        case DialogResult.Yes:

                            LogHelper.Debug(null, "User selected download");

                            Process.Start($"https://github.com/SresgaminG/ArmamutE/releases/download/{latestVersionValue}/GamemutE.msi");

                            return false;
                        case DialogResult.No:
                            return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.HandledException(null, ex);
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
                    return $"{array[0]}.{array[1]}.{array[2]}";

                return $"{array[0]}.{array[1]}";
            }
        }
    }
}
