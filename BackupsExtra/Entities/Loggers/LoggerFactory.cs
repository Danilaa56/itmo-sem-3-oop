using System;
using System.Collections.Generic;
using Backups.Tools;

namespace BackupsExtra.Entities.Loggers
{
    public static class LoggerFactory
    {
        private static bool _shutdown = false;
        private static Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>();

        public static Logger GetLogger(string label)
        {
            if (label is null)
                throw new ArgumentNullException(nameof(label));
            if (_shutdown)
                throw new BackupException("Logger was shutdown");
            if (_loggers.TryGetValue(label, out Logger existingLogger))
                return existingLogger;
            var logger = new Logger(label);
            _loggers[label] = logger;
            return logger;
        }

        // Shutdown all existing loggers
        public static void Shutdown()
        {
            foreach (Logger logger in _loggers.Values)
            {
                logger.Close();
            }
        }
    }
}