using System;

namespace Driven.Repl
{
    public class Timer
    {
        public Timer()
        {
            Start = DateTime.UtcNow;
        }

        public DateTime Start { get; private set; }
        public TimeSpan Interval { get { return DateTime.UtcNow - Start; } }

        public void Restart()
        {
            Start = DateTime.UtcNow;
        }
    }
}