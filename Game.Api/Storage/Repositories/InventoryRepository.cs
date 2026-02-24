using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Game.Api.Storage.Entities;

namespace Game.Api.Storage.Repositories
{
    public class InventoryRepository
    {
        private readonly TableClient _tableClient;
        public InventoryRepository(TableServiceClient svc, string tableName = "Inventories")
        {
            svc.CreateTableIfNotExists(tableName);
            _tableClient = svc.GetTableClient(tableName);
        }

        public async Task UpsertInventoryAsync(InventoryEntity ent)
        {
            await _tableClient.UpsertEntityAsync(ent);
        }

        public async Task<InventoryEntity> GetInventoryAsync(string playerId, string inventoryId)
        {
            var rk = $"{playerId}:{inventoryId}";
            try
            {
                var resp = await _tableClient.GetEntityAsync<InventoryEntity>("PLAYER_INV", rk);
                return resp.Value;
            }
            catch (RequestFailedException) { return null; }
        }
    }
}