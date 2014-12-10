using System;

namespace Driven
{
    public class SystemClock
    {
        static SystemClock()
        {
            Reset();
        }

        public static DateTimeOffset Now { get { return _utcNowFunc().ToLocalTime(); } }
        public static DateTimeOffset UtcNow { get { return _utcNowFunc(); } }

        private static Func<DateTimeOffset> _utcNowFunc;
        public static Func<DateTimeOffset> UtcNowFunc
        {
            set
            {
                var now = value();
                if (now.ToUniversalTime().Offset != now.Offset)
                {
                    throw new InvalidOperationException("UtcNowFunc must return a DateTimeOffset in the UTC Time Zone");
                }
                _utcNowFunc = value;
            }
        }

        public static void Reset()
        {
            _utcNowFunc = () => DateTimeOffset.UtcNow;
        }
    }
}