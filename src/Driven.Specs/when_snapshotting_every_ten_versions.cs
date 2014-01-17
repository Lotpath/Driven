using Xunit;

namespace Driven.Specs
{
    public class when_snapshotting_every_ten_versions
    {
        private readonly SnapshotCalculator _sut;

        public when_snapshotting_every_ten_versions()
        {
            _sut = new SnapshotCalculator(10);
        }

        [Fact]
        public void should_not_snapshot_after_one_commit()
        {
            var result = _sut.ShouldSnapshot(0, 1);
            Assert.False(result);
        }

        [Fact]
        public void should_not_snapshot_after_nine_commits()
        {
            var result = _sut.ShouldSnapshot(0, 9);
            Assert.False(result);
        }

        [Fact]
        public void should_snapshot_after_ten_commits()
        {
            var result = _sut.ShouldSnapshot(0, 10);
            Assert.True(result);
        }

        [Fact]
        public void should_snapshot_after_twenty_one_commits()
        {
            var result = _sut.ShouldSnapshot(0, 21);
            Assert.True(result);
        }

        [Fact]
        public void should_snapshot_at_twenty_one_commits_when_starting_at_ten()
        {
            var result = _sut.ShouldSnapshot(10, 21);
            Assert.True(result);
        }

        [Fact]
        public void should_snapshot_at_twenty_commits_when_starting_at_nineteen()
        {
            var result = _sut.ShouldSnapshot(19, 20);
            Assert.True(result);
        }

        [Fact]
        public void should_snapshot_at_thirty_one_commits_when_starting_at_nineteen()
        {
            var result = _sut.ShouldSnapshot(19, 31);
            Assert.True(result);
        }

        [Fact]
        public void should_not_snapshot_at_thirty_one_commits_when_starting_at_thirty()
        {
            var result = _sut.ShouldSnapshot(30, 31);
            Assert.False(result);
        }

        [Fact]
        public void should_not_snapshot_at_thirty_commits_when_starting_at_thirty()
        {
            var result = _sut.ShouldSnapshot(30, 30);
            Assert.False(result);
        }

        [Fact]
        public void should_not_snapshot_at_twenty_nine_commits_when_starting_at_twenty_eight()
        {
            var result = _sut.ShouldSnapshot(28, 29);
            Assert.False(result);
        }
    }   
}
