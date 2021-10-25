using System;

namespace Backups.Tools
{
    public class BackupException : Exception
    {
        public BackupException(string message, Exception e)
            : base(message, e)
        {
        }
    }
}