using System;

namespace BaseLibrary.Object
{
    public interface ILogEvent
    {
        event Action<string> LogEvent;
        LogLevel Level { get; set; }
        void Trace(string log);
        void Debug(string log);
        void Info(string log);
        void Warning(string log);
        void Error(string log);
    }

    public class LogEventObject : ILogEvent
    {
        public LogEventObject()
        {
            Level = LogLevel.Debug;
        }

        public event Action<string> LogEvent;

        public LogLevel Level { get; set; }

        #region log methods

        public void Trace(string log)
        {
            OnLogEvent(LogLevel.Trace, log);
        }

        public void Debug(string log)
        {
            OnLogEvent(LogLevel.Debug, log);
        }

        public void Info(string log)
        {
            OnLogEvent(LogLevel.Info, log);
        }


        public void Warning(string log)
        {
            OnLogEvent(LogLevel.Warning, log);
        }

        public void Error(string log)
        {
            OnLogEvent(LogLevel.Error, log);
        }

        #endregion

        protected void OnLogEvent(string log)
        {
            OnLogEvent(LogLevel.Info, log);
        }

        protected void OnLogEvent(LogLevel level, string log)
        {
            if (level >= Level)
            {
                var handler = LogEvent;
                handler?.Invoke($"[{level}]{log}");
            }
        }
    }

    public enum LogLevel
    {
        Trace = 0,
        Debug,
        Info,
        Warning,
        Error,
    }


    public class LogEventStaticObject
    {
        static LogEventStaticObject()
        {
            Level = LogLevel.Debug;
        }

        public static event Action<string> LogEvent;

        public static LogLevel Level { get; set; }

        #region log methods

        protected static void Trace(string log)
        {
            OnLogEvent(LogLevel.Trace, log);
        }

        protected static void Debug(string log)
        {
            OnLogEvent(LogLevel.Debug, log);
        }

        public static void Info(string log)
        {
            OnLogEvent(LogLevel.Info, log);
        }


        public static void Warning(string log)
        {
            OnLogEvent(LogLevel.Warning, log);
        }

        public static void Error(string log)
        {
            OnLogEvent(LogLevel.Error, log);
        }

        #endregion

        protected static void OnLogEvent(string log)
        {
            OnLogEvent(LogLevel.Info, log);
        }

        protected static void OnLogEvent(LogLevel level, string log)
        {
            if (level >= Level)
            {
                var handler = LogEvent;
                handler?.Invoke($"[{level}]{log}");
            }
        }
    }
}