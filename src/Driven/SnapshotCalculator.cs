using System;

namespace Driven
{
    internal class SnapshotCalculator
    {
        private readonly int _interval;

        public SnapshotCalculator(int interval)
        {
            _interval = interval;
        }

        public bool ShouldSnapshot(int originalVersion, int currentVersion)
        {
            var nextSnapshotThreshold =
                (Math.Floor((decimal)originalVersion / _interval) * _interval) + _interval;

            return currentVersion >= (int)nextSnapshotThreshold;
        }
    }
}