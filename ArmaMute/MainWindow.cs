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
        }

        private IKeyboardMouseEvents m_GlobalHook;

        private void SetupGlobalKeys()
        {
            Unsubscribe();
            Subscribe();
        }

        public void Subscribe()
        {
            // Note: for the application hook, use the Hook.AppEvents() instead
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.KeyUp += GlobalHookKeyUp;
        }

        public void Unsubscribe()
        {
            if (m_GlobalHook == null) return;

            m_GlobalHook.KeyUp -= GlobalHookKeyUp;

            //It is recommended to dispose
            m_GlobalHook.Dispose();
        }

        private void GlobalHookKeyUp(object sender, KeyEventArgs e)
        {
            Thread.Sleep(100);

            if (e.KeyCode == Keys.Multiply && ModifierKeys == Keys.Control)
                Mute.MuteUnmute("arma");
        }

        private void OnLoad(object sender, System.EventArgs e)
        {
            SetupGlobalKeys();

            this.WindowState = FormWindowState.Minimized;
            this.notifyIcon.ShowBalloonTip(2, "ARMA 3 Mute", "Use [CTRL + *] to mute ARMA 3", ToolTipIcon.Info);
        }

        private void OnResize(object sender, System.EventArgs e)
        {
            ShowInTaskbar = (this.WindowState != FormWindowState.Minimized);
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
