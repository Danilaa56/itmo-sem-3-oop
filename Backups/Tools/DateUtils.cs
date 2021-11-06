using System;

namespace Backups.Tools
{
    public static class DateUtils
    {
        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - DateTimeOffset.UnixEpoch).TotalMilliseconds;
        }
    }
}