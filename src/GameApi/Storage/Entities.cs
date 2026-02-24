using Azure;
using Azure.Data.Tables;

namespace GameApi.Storage
{
    public class PlayerEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        // custom props
        public string Location { get; set; }
        public int HP { get; set; }
        public string Inventory { get; set; }
        public string AvailableActions { get; set; }
    }

    public class ApiKeyEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string HashedKey { get; set; }
        public string CreatedAt { get; set; }
        public bool Revoked { get; set; }
    }
}
