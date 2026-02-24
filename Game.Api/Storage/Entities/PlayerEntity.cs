using System;
using Azure;
using Azure.Data.Tables;

namespace Game.Api.Storage.Entities
{
    public class PlayerEntity : ITableEntity
    {
        // PartitionKey: "PLAYER"
        // RowKey: playerId (string)
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        public string Handle { get; set; }
        public long Credits { get; set; }
        // JSON serialized reputation dictionary
        public string ReputationJson { get; set; }
        // JSON serialized list of ship IDs
        public string ShipsJson { get; set; }
        public string ActiveShipId { get; set; }
        // JSON serialized inventory entries
        public string InventoryJson { get; set; }
        // Skills stored as JSON
        public string SkillsJson { get; set; }
        // Last active game time as string YYYY.MM.DD.HH
        public string LastActiveGameTime { get; set; }
    }
}
