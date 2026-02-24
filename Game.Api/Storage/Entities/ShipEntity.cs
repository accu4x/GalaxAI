using System;
using Azure;
using Azure.Data.Tables;

namespace Game.Api.Storage.Entities
{
    public class ShipEntity : ITableEntity
    {
        // PartitionKey: "SHIP"
        // RowKey: shipId
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        public string OwnerPlayerId { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public bool Docked { get; set; }

        public int ShieldsCurrent { get; set; }
        public int ShieldsMax { get; set; }
        public int ArmorCurrent { get; set; }
        public int ArmorMax { get; set; }
        public int CapacitorCurrent { get; set; }
        public int CapacitorMax { get; set; }

        // JSON serialized modules and cargo
        public string ModulesJson { get; set; }
        public string CargoJson { get; set; }
        public double CargoMass { get; set; }
        public double CargoVolume { get; set; }
    }
}
