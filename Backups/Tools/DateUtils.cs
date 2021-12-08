using System;

namespace Backups.Tools
{
    public static class DateUtils
    {
        private static long _timeMillis;

        public static long CurrentTimeMillis()
        {
            return ++_timeMillis;
        }

        public static long RotateTime(long deltaTime)
        {
            if (deltaTime < 0)
                throw new ArgumentException("Delta time cannot be negative", nameof(deltaTime));
            return _timeMillis += deltaTime;
        }
    }
}