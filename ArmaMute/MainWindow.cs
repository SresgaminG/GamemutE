using System.Threading;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace SresgaminG.Arma3
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            LogHelper.Debug(this, "Starting ArmaMute");
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
            m_GlobalHook.KeyUp += GlobalHookKeyUp;

            LogHelper.Debug(this, "Subscribed to the global hook");
        }

        public void Unsubscribe()
        {
            if (m_GlobalHook == null) return;

            LogHelper.Debug(this, "Unsubscribing the global hook");

            m_GlobalHook.KeyUp -= GlobalHookKeyUp;

            //It is recommended to dispose
            m_GlobalHook.Dispose();

            LogHelper.Debug(this, "Global hook unsubscribed");
        }

        private void GlobalHookKeyUp(object sender, KeyEventArgs e)
        {
            LogHelper.Debug(this, "Global hook key up event");

            Thread.Sleep(100);

            if (e.KeyCode == Keys.Multiply && ModifierKeys == Keys.Control)
            {
                LogHelper.Debug(this, "Correct key combo pressed");

                Mute.MuteUnmute("arma");
            }
        }

        private void OnLoad(object sender, System.EventArgs e)
        {
            this.Text = string.Format("{0} - v{1}", this.Text, Application.ProductVersion);
            SetupGlobalKeys();

            this.WindowState = FormWindowState.Minimized;
            this.Visible = this.ShowInTaskbar = false;
            this.notifyIcon.ShowBalloonTip(2, "ARMA 3 Mute", "Use [CTRL + *] to mute ARMA 3", ToolTipIcon.Info);
        }

        private void OnResize(object sender, System.EventArgs e)
        {
            Visible = ShowInTaskbar = (this.WindowState != FormWindowState.Minimized);
        }

        private void OnNotifyDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;

            this.Activate();
        }

        private void OnClickExit(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
