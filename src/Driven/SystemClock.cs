using System;

namespace Driven
{
    public class SystemClock
    {
        static SystemClock()
        {
            Reset();
        }

        public static DateTimeOffset Now { get { return UtcNowFunc().ToLocalTime(); } }
        public static DateTimeOffset UtcNow { get { return UtcNowFunc(); } }
        public static Func<DateTimeOffset> UtcNowFunc;

        public static void Reset()
        {
            UtcNowFunc = () => DateTimeOffset.UtcNow;
        }
    }
}