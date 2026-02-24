using System;

namespace Game.Api.Services
{
    public class GameTimeService
    {
        // Epoch mapping and acceleration parameters
        private readonly DateTime _realEpoch = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly DateTime _gameEpoch = new DateTime(3000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // By default, 1 real hour -> 1 game day
        private readonly double _realSecondsPerGameDay = 3600.0; // can be configured later

        public GameTimeService()
        {
        }

        public DateTime NowInGameUtc()
        {
            var now = DateTime.UtcNow;
            var realElapsed = (now - _realEpoch).TotalSeconds;
            var gameDaysElapsed = realElapsed / _realSecondsPerGameDay;
            var gameElapsedSeconds = gameDaysElapsed * 24 * 3600.0; // seconds in game elapsed
            return _gameEpoch.AddSeconds(gameElapsedSeconds);
        }

        public string NowInGameString()
        {
            var g = NowInGameUtc();
            return g.ToString("yyyy.MM.dd.HH");
        }
    }
}