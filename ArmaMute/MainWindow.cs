using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Gma.System.MouseKeyHook;
using SresgaminG.GamemutE.Entities;
using SresgaminG.GamemutE.Helpers;

namespace SresgaminG.GamemutE
{
    public partial class MainWindow : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        
        private IKeyboardMouseEvents m_GlobalHook;
        private Keys Binding = Keys.F1;
        
        private const string updateLink = "http://armamute.sresgaming.com/armamute_start_ping.php";
        
        private string settingsXml = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/SresgaminG/settings.xml";

        private BindingList<Game> gameList = new BindingList<Game>();
        BindingList<Tuple<string, string>> exeWindows = new BindingList<Tuple<string, string>>();

        public MainWindow()
        {
            InitializeComponent();

            LogHelper.Info(this, "Starting GamemutE");

            ShowBinding();
                        
            SendPingToSite();
        }

        private void SendPingToSite()
        {
            Task task = new Task(() =>
              {
                  try
                  {
                      LogHelper.Debug(this, "Pinging website");

                      HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(updateLink);

                      using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                      {
                          StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                          string reply = reader.ReadToEnd();

                          LogHelper.Debug(this, $"Website replied: {reply}");
                      }
                  }
                  catch (Exception ex)
                  {
                      LogHelper.HandledException(this, ex);
                  }
              });

            task.Start();
        }

        private void SetupGlobalKeys()
        {
            Unsubscribe();
            Subscribe();
        }

        public void Subscribe()
        {
            LogHelper.Debug(this, "Subscribing to the global hook");

            // Note: for the application hook, use the Hook.AppEvents() instead
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.KeyDown += GlobalHookKeyDown;

            LogHelper.Debug(this, "Subscribed to the global hook");
        }

        public void Unsubscribe()
        {
            if (m_GlobalHook == null) return;

            LogHelper.Debug(this, "Unsubscribing the global hook");

            m_GlobalHook.KeyDown -= GlobalHookKeyDown;

            //It is recommended to dispose
            m_GlobalHook.Dispose();

            LogHelper.Debug(this, "Global hook unsubscribed");
        }

        private void GlobalHookKeyDown(object sender, KeyEventArgs e)
        {
            new Thread(CallMuteThread).Start(e);
        }

        private void CallMuteThread(object parameter)
        {
            KeyEventArgs e = (parameter as KeyEventArgs);
            if ((e.KeyCode | ModifierKeys) == Binding)
            {
                IntPtr handle = GetForegroundWindow();
                uint processId = 0;
                GetWindowThreadProcessId(handle, out processId);

                Process process = Process.GetProcessById(Convert.ToInt32(processId));

                Mute.MuteUnmute(process.ProcessName);
            }
        }

        private void OnLoad(object sender, EventArgs e)
        {
            this.Text = $"{this.Text} {Program.ApplicationVersion}";
            SetupGlobalKeys();

            this.WindowState = FormWindowState.Minimized;
            this.Visible = this.ShowInTaskbar = false;
            this.notifyIcon.ShowBalloonTip(2, $"GamemutE {Program.ApplicationVersion}", $"Use [{GetBindingText(Binding.ToString())}] to mute a game", ToolTipIcon.Info);
            this.notifyIcon.Text = $"GamemutE {Program.ApplicationVersion}";

            saveButton.Focus();
        }

        private void OnResize(object sender, EventArgs e)
        {
            Visible = ShowInTaskbar = (this.WindowState != FormWindowState.Minimized);
        }

        private void OnNotifyDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.Visible = this.ShowInTaskbar = true;
            }

            this.Activate();
        }

        private void OnClickExit(object sender, EventArgs e)
        {
            Close();
        }

        #region Keybinding Changes

        private bool bindingChanged = false;

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            Binding = e.Modifiers | e.KeyCode;
            bindingChanged = true;
            ShowBinding();
        }

        private void ShowBinding()
        {
            muteSelect.Text = GetBindingText(Binding.ToString());
            saveButton.Focus();
        }

        private static string GetBindingText(string globalBinding)
        {
            string[] binding = globalBinding.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            Array.Reverse(binding);

            return string.Join("+", binding);
        }

        #endregion

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Suppress the charactere being entered into the text box
            e.SuppressKeyPress = true;
        }

        private void OnEnter(object sender, EventArgs e)
        {
            bindingChanged = false;
            muteSelect.Clear();
        }

        private void OnLeave(object sender, EventArgs e)
        {
            if (!bindingChanged)
                ShowBinding();
        }

        private void OnLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.twitch.tv/sresgaming");
        }

        private void OnDonateClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://twitch.streamlabs.com/sresgaming#/");
        }

        private void OnClickSave(object sender, EventArgs e)
        {
            CreateXML(Binding.ToString());
        }

        public static bool CreateXML(string binding)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                string strXML = "<sresgaming><bind>{0}</bind></sresgaming>";

                xmlDoc.LoadXml(string.Format(strXML, binding));

                string sresGamingFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SresgaminG";

                if (!Directory.Exists(sresGamingFolder))
                    Directory.CreateDirectory(sresGamingFolder);

                xmlDoc.Save(sresGamingFolder + "/settings.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GamemutE", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            MessageBox.Show(string.Format("Key binding {0} has been saved", GetBindingText(binding)), "GamemutE", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return true;
        }
    }
}
