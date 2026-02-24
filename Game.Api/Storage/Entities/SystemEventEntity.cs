using Azure;
using Azure.Data.Tables;
using System;

namespace Game.Api.Storage.Entities
{
    public class SystemEventEntity : ITableEntity
    {
        // PartitionKey = "SYSTEM_EVENT", RowKey = "{systemId}:{eventId}" (eventId = timestamp or guid)
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        public string SystemId { get; set; }
        public string EventJson { get; set; }
    }
}