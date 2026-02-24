using Azure.Data.Tables;

namespace GameApi.Storage
{
    public class AzureTableStore : ITableStore
    {
        private readonly TableClient _players;
        private readonly TableClient _keys;
        public AzureTableStore(Microsoft.Extensions.Options.IOptions<GameApi.Settings> settings)
        {
            var conn = settings.Value.AzureTablesConnectionString;
            _players = new TableClient(conn, settings.Value.TableNames_Players);
            _keys = new TableClient(conn, settings.Value.TableNames_Keys);
            _players.CreateIfNotExists();
            _keys.CreateIfNotExists();
        }

        public async Task<PlayerEntity> GetPlayerAsync(string playerId)
        {
            try
            {
                var resp = await _players.GetEntityAsync<PlayerEntity>("world", playerId);
                var e = resp.Value;
                e.ETag = resp.GetRawResponse().Headers.ETag?.ToString();
                return e;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404) { return null; }
        }

        public async Task UpsertPlayerAsync(PlayerEntity player)
        {
            await _players.UpsertEntityAsync(player, TableUpdateMode.Replace, player.ETag == null ? default : new Azure.ETag(player.ETag));
        }

        public async Task InsertApiKeyAsync(ApiKeyEntity key)
        {
            await _keys.AddEntityAsync(key);
        }

        public async Task<IEnumerable<ApiKeyEntity>> ListApiKeysAsync(string userId)
        {
            var q = _keys.Query<ApiKeyEntity>(e => e.PartitionKey == userId);
            return q.ToList();
        }

        public async Task RevokeApiKeyAsync(string keyId)
        {
            // fetch and set revoked flag
            var ent = await _keys.Query<ApiKeyEntity>(e => e.Id == keyId).FirstOrDefaultAsync();
            if (ent != null) { ent.Revoked = true; await _keys.UpdateEntityAsync(ent, ent.ETag); }
        }

        public async Task RevokeAllForUserAsync(string userId)
        {
            var items = _keys.Query<ApiKeyEntity>(e => e.PartitionKey == userId).ToList();
            foreach (var it in items) { it.Revoked = true; await _keys.UpdateEntityAsync(it, it.ETag); }
        }

        public async Task<ApiKeyEntity> GetApiKeyByHashAsync(string hash)
        {
            var it = _keys.Query<ApiKeyEntity>(e => e.HashedKey == hash && e.Revoked == false).FirstOrDefault();
            return it;
        }
    }
}
