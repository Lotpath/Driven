using SubSpec;
using Xunit;

namespace Driven.Specs
{
    public class SnapshotCalculatorTests
    {
        [Specification]
        public void TenCommitIntervalTests()
        {
            var sut = default(SnapshotCalculator);

            "Given a snapshot calculator with an interval of 10 commits"
                .Context(() => sut = new SnapshotCalculator(10));

            "then should not snapshot after a single commit with no prior commits"
                .Assert(() => Assert.False(sut.ShouldSnapshot(0, 1)));

            "then should not snapshot after nine commits with no prior commits"
                .Assert(() => Assert.False(sut.ShouldSnapshot(0, 9)));

            "then should snapshot after ten commits with no prior commits"
                .Assert(() => Assert.True(sut.ShouldSnapshot(0, 10)));

            "then should snapshot after 21 commits with no prior commits"
                .Assert(() => Assert.True(sut.ShouldSnapshot(0, 21)));

            "then should shapshot after 21 commits with 10 prior commits"
                .Assert(() => Assert.True(sut.ShouldSnapshot(10, 21)));

            "then should shapshot after 20 commits with 19 prior commits"
                .Assert(() => Assert.True(sut.ShouldSnapshot(19, 20)));

            "then should shapshot after 31 commits with 19 prior commits"
                .Assert(() => Assert.True(sut.ShouldSnapshot(19, 31)));

            "then should not shapshot after 31 commits with 30 prior commits"
                .Assert(() => Assert.False(sut.ShouldSnapshot(30, 31)));

            "then should not shapshot after 30 commits with 30 prior commits"
                .Assert(() => Assert.False(sut.ShouldSnapshot(30, 30)));

            "then should not shapshot after 29 commits with 28 prior commits"
                .Assert(() => Assert.False(sut.ShouldSnapshot(28, 29)));
        }
    }   
}
