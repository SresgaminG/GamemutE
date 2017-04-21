using System;
using System.Diagnostics;
using NLog;

namespace SresgaminG.GamemutE.Helpers
{
    public static class LogHelper
    {
        #region Public methods
        public static void Debug(object sender, string message, params object[] args)
        {
            LogMessage(sender, LoggingLevel.Debug, message, args);
        }

        public static void Info(object sender, string message, params object[] args)
        {
            LogMessage(sender, LoggingLevel.Info, message, args);
        }

        public static void Warning(object sender, string message, params object[] args)
        {
            LogMessage(sender, LoggingLevel.Warning, message, args);
        }

        public static void Error(object sender, string message, params object[] args)
        {
            LogMessage(sender, LoggingLevel.Error, message, args);
        }

        public static void UnhandledException(object sender, Exception ex)
        {
            LogException(sender, LoggingLevel.Critical, ex);
        }

        public static void HandledException(object sender, Exception ex)
        {
            LogException(sender, LoggingLevel.Critical, ex);
        }
        #endregion

        #region Private enumeration and methods
        private enum LoggingLevel
        {
            Critical,
            Error,
            Warning,
            Info,
            Debug
        }

        private static void LogException(object sender, LoggingLevel loggingLevel, Exception ex)
        {
            Exception exceptionToLog = ex;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            LogMessage(sender, loggingLevel, string.Format("Exception thrown - {0}.{1} Stacktrace - {2}", ex.Message, Environment.NewLine, ex.StackTrace));
        }

        private static void LogMessage(object sender, LoggingLevel loggingLevel, string message, params object[] args)
        {
            try
            {
                string loggerName = "Main";
                if (sender != null)
                {
                    loggerName = sender.GetType().Name;
                }

                message = string.Format(message, args);

                Logger logger = LogManager.GetLogger(loggerName);
                switch (loggingLevel)
                {
                    case LoggingLevel.Error: logger.Error(message); break;
                    case LoggingLevel.Warning: logger.Warn(message); break;
                    case LoggingLevel.Info: logger.Info(message); break;
                    case LoggingLevel.Critical: logger.Fatal(message); break;
                    default: logger.Debug(message); break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("NLog exception: {0}, {1}", ex.Message, ex.StackTrace));
            }
        }
        #endregion
    }
}
