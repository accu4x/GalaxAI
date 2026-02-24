using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;

namespace Game.Storage.Adapters
{
    public class TableStorageAdapter
    {
        private readonly TableServiceClient _serviceClient;
        private readonly string _tableName;

        public TableStorageAdapter(string connectionString, string tableName = "GameEntities")
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            _serviceClient = new TableServiceClient(connectionString);
            _tableName = tableName;
            EnsureTableExistsAsync().GetAwaiter().GetResult();
        }

        private async Task EnsureTableExistsAsync()
        {
            var client = _serviceClient.GetTableClient(_tableName);
            await client.CreateIfNotExistsAsync();
        }

        public async Task<TableEntity?> GetEntityAsync(string partitionKey, string rowKey)
        {
            var client = _serviceClient.GetTableClient(_tableName);
            try
            {
                var response = await client.GetEntityAsync<TableEntity>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task UpsertEntityAsync(TableEntity entity, ETag? ifMatch = null)
        {
            var client = _serviceClient.GetTableClient(_tableName);
            if (ifMatch.HasValue)
            {
                await client.UpdateEntityAsync(entity, ifMatch.Value, TableUpdateMode.Replace);
            }
            else
            {
                await client.UpsertEntityAsync(entity, TableUpdateMode.Merge);
            }
        }

        public async Task DeleteEntityAsync(string partitionKey, string rowKey)
        {
            var client = _serviceClient.GetTableClient(_tableName);
            await client.DeleteEntityAsync(partitionKey, rowKey);
        }

        public async Task<IEnumerable<TableEntity>> QueryAsync(string filter = "")
        {
            var client = _serviceClient.GetTableClient(_tableName);
            var entities = new List<TableEntity>();
            await foreach (var e in client.QueryAsync<TableEntity>(filter))
            {
                entities.Add(e);
            }
            return entities;
        }
    }
}
