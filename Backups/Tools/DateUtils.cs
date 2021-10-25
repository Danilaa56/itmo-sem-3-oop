using System;

namespace Backups.Tools
{
    public static class DateUtils
    {
        private static readonly DateTime UtcBegin = new DateTime(1970, 1, 1, 0, 0, 0);

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - UtcBegin).TotalMilliseconds;
        }
    }
}