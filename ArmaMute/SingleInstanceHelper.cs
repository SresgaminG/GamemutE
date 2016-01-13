using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace SresgaminG.Arma3
{
    public class SingleInstanceHelper
    {
        #region Enumerated Constants

        public enum SingleInstanceScope
        {
            CurrentUser,
            AllUsers
        }

        #endregion

        #region Fields

        public readonly Int32 WM_SHOWFIRSTINSTANCE;

        private Mutex mutex;

        #endregion 

        #region Constructor

        public SingleInstanceHelper()
        {
            WM_SHOWFIRSTINSTANCE = WindowApiHelper.RegisterWindowMessage("WM_SHOWFIRSTINSTANCE|{0}", AssemblyIdentifier());
        }

        #endregion

        #region Methods

        private String AssemblyIdentifier()
        {
            GuidAttribute attribute;

            //Get first attributes of required type
            attribute = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false).FirstOrDefault() as GuidAttribute;

            //If not found return empty string
            if (attribute == null) return String.Empty;

            //Return first attribute
            return attribute.Value;
        }

        public Boolean Start(SingleInstanceScope scope)
        {
            Boolean firstInstance = false;
            String singleInstanceName;

            //Local limits instance to one per logged in user
            //Global limits scope to 1 across all user sessions
            singleInstanceName = String.Format("{0}\\{1}", scope == SingleInstanceScope.CurrentUser ? "Local" : "Global", AssemblyIdentifier());

            //Create mutex and get whether this is first instance
            mutex = new Mutex(true, singleInstanceName, out firstInstance);

            //Return whether this is first instance
            return firstInstance;
        }

        public void ShowFirstInstance()
        {
            WindowApiHelper.PostMessage((IntPtr)WindowApiHelper.HWND_BROADCAST, WM_SHOWFIRSTINSTANCE, IntPtr.Zero, IntPtr.Zero);
        }

        public void Stop()
        {
            //Release mutex
            mutex.ReleaseMutex();
        }

        #endregion
    }
}
