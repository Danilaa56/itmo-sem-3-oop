using System;

namespace Backups.Server.Tools
{
    public class Logger
    {
        public Logger(string label)
        {
            Label = label ?? throw new ArgumentNullException(nameof(label));
        }

        public string Label { get; }

        public void Info(object message)
        {
            Console.WriteLine($"[{Label}]: {message}");
        }
        
        public void Error(object message)
        {
            Console.Error.WriteLine($"[{Label}]: {message}");
        }
    }
}