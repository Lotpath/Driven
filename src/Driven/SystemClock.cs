using System;

namespace Driven
{
    public class SystemClock
    {
        static SystemClock()
        {
            Reset();
        }

        public static DateTime Now { get { return UtcNowFunc().ToLocalTime(); } }
        public static DateTime UtcNow { get { return UtcNowFunc(); } }
        public static Func<DateTime> UtcNowFunc;

        public static void Reset()
        {
            UtcNowFunc = () => DateTime.UtcNow;
        }
    }
}