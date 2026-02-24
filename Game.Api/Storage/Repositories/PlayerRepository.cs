using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Game.Api.Storage.Entities;

namespace Game.Api.Storage.Repositories
{
    public class PlayerRepository
    {
        private readonly TableClient _table;

        public PlayerRepository(AzureTableClient client)
        {
            _table = client.GetTableClient("Players");
        }

        public async Task<PlayerEntity> GetPlayerAsync(string playerId)
        {
            try
            {
                var resp = await _table.GetEntityAsync<PlayerEntity>("PLAYER", playerId);
                return resp.Value;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        public async Task UpsertPlayerAsync(PlayerEntity p)
        {
            await _table.UpsertEntityAsync(p);
        }
    }
}
