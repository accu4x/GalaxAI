using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Game.Api.Storage.Entities;
using Game.Api.Combat;

namespace Game.Api.Storage.Repositories
{
    public class CombatSessionRepository
    {
        private readonly TableClient _table;

        public CombatSessionRepository(AzureTableClient client)
        {
            _table = client.GetTableClient("CombatSessions");
        }

        public async Task<CombatSession> GetAsync(string sessionId)
        {
            try
            {
                var resp = await _table.GetEntityAsync<CombatSessionEntity>("COMBAT", sessionId);
                var e = resp.Value;
                return JsonSerializer.Deserialize<CombatSession>(e.SessionJson);
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        public async Task UpsertAsync(CombatSession session)
        {
            var entity = new CombatSessionEntity
            {
                PartitionKey = "COMBAT",
                RowKey = session.SessionId,
                LastUpdated = session.LastUpdated,
                SessionJson = JsonSerializer.Serialize(session)
            };
            await _table.UpsertEntityAsync(entity);
        }
    }
}
