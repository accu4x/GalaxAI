using System.Collections.Generic;

namespace Game.Mock.Data
{
    public static class SeedData
    {
        public static IEnumerable<object> Players => new[] {
            new { PlayerId = "player1", Location = "Hangar Bay", Hp = 100 }
        };
    }
}
