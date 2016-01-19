using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Gma.System.MouseKeyHook;
using SresgaminG.Arma3;

namespace SresgaminG.Arma
{
    public partial class MainWindow : Form
    {
        private Keys Binding = Keys.Control | Keys.Multiply;

        public MainWindow()
        {
            InitializeComponent();

            LogHelper.Info(this, "Starting ArmamutE");

            LogHelper.Info(this, "Looking for settings file");
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SresgaminG/settings.xml"))
            {
                LogHelper.Info(this, "Settings file found");

                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SresgaminG/settings.xml");

                    LogHelper.Info(this, "Looking for bind setting");
                    if (xmlDoc.SelectSingleNode("sresgaming/bind") != null)
                    {
                        LogHelper.Info(this, "Found bind setting");

                        Binding = Keys.None;
                        string tempBinding = xmlDoc.SelectSingleNode("sresgaming/bind").InnerText;

                        string[] bindSplit = tempBinding.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        LogHelper.Info(this, "Parsing bind setting");
                        foreach (string value in bindSplit)
                        {
                            Keys key = (Keys)Enum.Parse(typeof(Keys), value);

                            Binding = Binding | key;
                        }

                        LogHelper.Info(this, "Bind is good - {0}", GetBindingText(Binding.ToString()));
                    }
                }
                catch
                {
                    LogHelper.Error(this, "Unable to open settings folder, reseting to default key bind");
                    Binding = Keys.Control | Keys.Multiply;
                }
            }

            ShowBinding();
        }

        private IKeyboardMouseEvents m_GlobalHook;

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
                Mute.MuteUnmute("arma");
        }

        private void OnLoad(object sender, EventArgs e)
        {
            this.Text = string.Format("{0} - v{1}", this.Text, Program.ApplicationVersion);
            SetupGlobalKeys();

            this.WindowState = FormWindowState.Minimized;
            this.Visible = this.ShowInTaskbar = false;
            this.notifyIcon.ShowBalloonTip(2, string.Format("ArmamutE v{0}", Program.ApplicationVersion), string.Format("Use [{0}] to mute ARMA 3", GetBindingText(Binding.ToString())), ToolTipIcon.Info);

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
                MessageBox.Show(ex.Message, "ArmamutE", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            MessageBox.Show(string.Format("Key binding {0} has been saved", GetBindingText(binding)), "ArmamutE", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return true;
        }
    }
}
