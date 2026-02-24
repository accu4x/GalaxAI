using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure;
using BCrypt.Net;

namespace Game.Auth.Services
{
    public class CreateKeyResult
    {
        public string RawKey { get; set; } = null!;
        public string KeyId { get; set; } = null!;
    }

    public class ApiKeyService
    {
        private readonly TableServiceClient _tableServiceClient;
        private readonly string _tableName = "ApiKeys";

        public ApiKeyService(string storageConnectionString)
        {
            if (string.IsNullOrWhiteSpace(storageConnectionString)) throw new ArgumentNullException(nameof(storageConnectionString));
            _tableServiceClient = new TableServiceClient(storageConnectionString);
            EnsureTableExistsAsync().GetAwaiter().GetResult();
        }

        private async Task EnsureTableExistsAsync()
        {
            var client = _tableServiceClient.GetTableClient(_tableName);
            await client.CreateIfNotExistsAsync();
        }

        // Create a new raw key (returned once) and store only a salted hash in Table Storage.
        public async Task<CreateKeyResult> CreateKeyAsync(string userId, string? note = null)
        {
            var rawKey = GenerateSecureKey();
            var hashed = BCrypt.Net.BCrypt.HashPassword(rawKey);
            var keyId = Guid.NewGuid().ToString("N");

            var client = _tableServiceClient.GetTableClient(_tableName);
            var entity = new TableEntity(userId, keyId)
            {
                { "Hash", hashed },
                { "CreatedAt", DateTime.UtcNow },
                { "Revoked", false }
            };
            if (!string.IsNullOrWhiteSpace(note)) entity.Add("Note", note);

            await client.AddEntityAsync(entity);
            return new CreateKeyResult { RawKey = rawKey, KeyId = keyId };
        }

        public async Task<IEnumerable<TableEntity>> ListKeysAsync(string userId)
        {
            var client = _tableServiceClient.GetTableClient(_tableName);
            var filter = TableClient.CreateQueryFilter($"PartitionKey eq '{userId}'");
            var results = new List<TableEntity>();
            await foreach (var e in client.QueryAsync<TableEntity>(filter)) results.Add(e);
            return results;
        }

        public async Task<bool> VerifyKeyAsync(string rawKey)
        {
            // Iterate all keys and compare - for MVP this is acceptable; optimize later with token prefixing/indexing.
            var client = _tableServiceClient.GetTableClient(_tableName);
            await foreach (var e in client.QueryAsync<TableEntity>())
            {
                if (e.TryGetValue("Hash", out var hashObj) && hashObj is string hash)
                {
                    if (BCrypt.Net.BCrypt.Verify(rawKey, hash)) return true;
                }
            }
            return false;
        }

        public async Task RevokeKeyAsync(string userId, string rowKey)
        {
            var client = _tableServiceClient.GetTableClient(_tableName);
            await client.DeleteEntityAsync(userId, rowKey);
        }

        private string GenerateSecureKey(int bytes = 32)
        {
            using var rng = RandomNumberGenerator.Create();
            var buffer = new byte[bytes];
            rng.GetBytes(buffer);
            return Convert.ToBase64String(buffer).Replace("=", "").Replace("+", "").Replace("/", "");
        }
    }
}
