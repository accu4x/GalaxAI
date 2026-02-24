using System;
using System.Collections.Generic;

namespace Game.Api.Combat
{
    public class CombatSession
    {
        public string SessionId { get; set; }
        public ShipState Player { get; set; }
        public ShipState Enemy { get; set; }
        public CombatConfig Config { get; set; }
        public int Turn { get; set; }
        public int ThreatClock { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
    }
}