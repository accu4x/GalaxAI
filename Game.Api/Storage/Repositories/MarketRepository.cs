using System.Threading.Tasks;
using Azure.Data.Tables;
using Game.Api.Storage.Entities;

namespace Game.Api.Storage.Repositories
{
    public class MarketRepository
    {
        private readonly TableClient _table;

        public MarketRepository(AzureTableClient client, string systemId)
        {
            _table = client.GetTableClient($"Market_{systemId}");
        }

        public async Task<MarketEntity> GetItemAsync(string itemId)
        {
            try
            {
                var resp = await _table.GetEntityAsync<MarketEntity>($"MARKET_{_table.Name}", itemId);
                return resp.Value;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        public async Task UpsertItemAsync(MarketEntity e)
        {
            await _table.UpsertEntityAsync(e);
        }
    }
}
