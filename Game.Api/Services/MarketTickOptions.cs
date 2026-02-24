using System;

namespace Game.Api.Services
{
    public class MarketTickOptions
    {
        // Real-world minutes per game-day tick. Configurable.
        // Default: 60 (1 real hour = 1 game day)
        public int RealMinutesPerGameDay { get; set; } = 60;

        // Allow accelerated dev mode multiplier (e.g. 0.1 => 10x faster ticks)
        public double DevMultiplier { get; set; } = 1.0;

        // Randomness amplitude for price tick (fractional, e.g. 0.05 = +/-5%)
        public double PriceRandomness { get; set; } = 0.05;
    }
}