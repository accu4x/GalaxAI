using System;
using Game.Api.Services;
using Xunit;

namespace Game.Api.Tests
{
    public class GameTimeServiceTests
    {
        [Fact]
        public void NowInGameString_FormatsCorrectly()
        {
            var svc = new GameTimeService();
            var now = svc.NowInGameString();
            Assert.Matches(@"^\d{4}\.\d{2}\.\d{2}\.\d{2}$", now);
        }
    }
}