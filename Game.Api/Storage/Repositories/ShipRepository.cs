using System.Threading.Tasks;
using Azure.Data.Tables;
using Game.Api.Storage.Entities;

namespace Game.Api.Storage.Repositories
{
    public class ShipRepository
    {
        private readonly TableClient _table;

        public ShipRepository(AzureTableClient client)
        {
            _table = client.GetTableClient("Ships");
        }

        public async Task<ShipEntity> GetShipAsync(string shipId)
        {
            try
            {
                var resp = await _table.GetEntityAsync<ShipEntity>("SHIP", shipId);
                return resp.Value;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        public async Task UpsertShipAsync(ShipEntity s)
        {
            await _table.UpsertEntityAsync(s);
        }
    }
}
