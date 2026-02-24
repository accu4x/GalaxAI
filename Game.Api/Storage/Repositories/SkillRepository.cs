using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Game.Api.Storage.Entities;

namespace Game.Api.Storage.Repositories
{
    public class SkillRepository
    {
        private readonly TableClient _tableClient;
        public SkillRepository(TableServiceClient svc, string tableName = "Skills")
        {
            svc.CreateTableIfNotExists(tableName);
            _tableClient = svc.GetTableClient(tableName);
        }

        public async Task UpsertSkillAsync(SkillEntity ent)
        {
            await _tableClient.UpsertEntityAsync(ent);
        }

        public async Task<SkillEntity> GetSkillAsync(string playerId, string skillId)
        {
            var rk = $"{playerId}:{skillId}";
            try
            {
                var resp = await _tableClient.GetEntityAsync<SkillEntity>("PLAYER_SKILL", rk);
                return resp.Value;
            }
            catch (RequestFailedException) { return null; }
        }
    }
}