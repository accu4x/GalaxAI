using System;
using Azure;
using Azure.Data.Tables;

namespace Game.Api.Storage.Entities
{
    public class CombatSessionEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string SessionJson { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}