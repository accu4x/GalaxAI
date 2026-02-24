using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Game.Api.Storage.Entities;

namespace Game.Api.Storage.Repositories
{
    public class SystemEventRepository
    {
        private readonly TableClient _tableClient;
        public SystemEventRepository(TableServiceClient svc, string tableName = "SystemEvents")
        {
            svc.CreateTableIfNotExists(tableName);
            _tableClient = svc.GetTableClient(tableName);
        }

        public async Task UpsertEventAsync(SystemEventEntity ent)
        {
            await _tableClient.UpsertEntityAsync(ent);
        }
    }
}