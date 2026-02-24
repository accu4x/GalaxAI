using System;
using Azure;
using Azure.Data.Tables;

namespace Game.Api.Storage.Entities
{
    public class MarketEntity : ITableEntity
    {
        // PartitionKey: "MARKET_{systemId}"
        // RowKey: itemId
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        public string ItemId { get; set; }
        public double Price { get; set; }
        public double Supply { get; set; }
        public double Demand { get; set; }
        // last updated game time
        public string LastUpdateGameTime { get; set; }
    }
}
