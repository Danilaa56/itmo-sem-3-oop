using System;

namespace Backups.Tools
{
    public class DateUtils
    {
        private static readonly DateTime utcBegin = new DateTime(1970, 1, 1, 0, 0, 0);

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - utcBegin).TotalMilliseconds;
        }
    }
}