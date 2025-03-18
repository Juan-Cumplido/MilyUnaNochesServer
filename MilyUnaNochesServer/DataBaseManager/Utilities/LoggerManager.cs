using log4net;
using System;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace MilyUnaNochesService.Utilities
{
    public class LoggerManager
    {
        public ILog Logger { get; set; }

        public LoggerManager(Type type)
        {
            Logger = LogManager.GetLogger(type);
        }

        public ILog GetLogger(Type type)
        {
            return LogManager.GetLogger(type);
        }

        public void LogInfo(string message)
        {
            Logger.Info(message);
        }

        public void LogError(Exception exception)
        {
            Logger.Error(exception);
        }
        public void LogError(string message, Exception exception) {
            Logger.Error(message, exception);
        }

        public void LogFatal(Exception exception)
        {
            Logger.Fatal(exception);
        }

        public void LogWarn(Exception exception)
        {
            Logger.Warn(exception);
        }

        public void LogDebug(Exception exception)
        {
            Logger.Debug(exception);
        }

    }
}
