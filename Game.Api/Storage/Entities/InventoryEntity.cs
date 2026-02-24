using Azure;
using Azure.Data.Tables;
using System;

namespace Game.Api.Storage.Entities
{
    public class InventoryEntity : ITableEntity
    {
        // PartitionKey = "PLAYER_INV", RowKey = "{playerId}:{inventoryId}" (inventoryId can be PERSONAL or SHIP:{shipId})
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        public string PlayerId { get; set; }
        public string InventoryId { get; set; }

        // JSON payload for items
        public string InventoryJson { get; set; }
    }
}