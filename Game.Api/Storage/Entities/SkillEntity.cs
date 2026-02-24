using Azure;
using Azure.Data.Tables;
using System;

namespace Game.Api.Storage.Entities
{
    public class SkillEntity : ITableEntity
    {
        // PartitionKey = "PLAYER_SKILL", RowKey = "{playerId}:{skillId}"
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        // JSON blob payload
        public string SkillJson { get; set; }

        // convenience fields
        public string PlayerId { get; set; }
        public string SkillId { get; set; }
    }
}