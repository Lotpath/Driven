using System;
using System.Linq;
using NEventStore;
using Xunit;

namespace Driven.Testing
{
    public class Result
    {
        public Exception ThrownException { get; set; }
        public Commit Commit { get; set; }
    }

    public static class ResultExtensions
    {
        public static void ShouldHaveEventOf<T>(this Result result)
        {
            var match = false;
            var events = result.Commit.Events.Select(x => x.Body).ToList();
            foreach (var e in events)
            {
                if (e.GetType().IsAssignableFrom(typeof (T)))
                    match = true;
            }
            Assert.True(match, "Expected event of type " + typeof(T));
        }
    }
}