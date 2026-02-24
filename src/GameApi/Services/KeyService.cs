using System.Security.Cryptography;
using System.Text;
using GameApi.Models;

namespace GameApi.Services
{
    public class KeyService
    {
        private readonly Storage.ITableStore _store;
        public KeyService(Storage.ITableStore store) { _store = store; }

        public async Task<Models.ApiKey> CreateKeyAsync(string userId)
        {
            var key = GenerateKey();
            var hashed = HashKey(key);
            var model = new Storage.ApiKeyEntity { Id = Guid.NewGuid().ToString(), UserId = userId, HashedKey = hashed, CreatedAt = DateTime.UtcNow.ToString("o"), Revoked = false };
            await _store.InsertApiKeyAsync(model);
            return new Models.ApiKey { Id = model.Id, PlaintextKey = key };
        }

        public async Task<IEnumerable<Models.ApiKey>> ListKeysAsync(string userId)
        {
            var ents = await _store.ListApiKeysAsync(userId);
            return ents.Select(e => new Models.ApiKey { Id = e.Id, CreatedAt = e.CreatedAt, Revoked = e.Revoked });
        }

        public async Task RevokeKeyAsync(string keyId)
        {
            await _store.RevokeApiKeyAsync(keyId);
        }

        public async Task<Models.ApiKey> RotateKeyAsync(string userId)
        {
            // revoke existing keys for user
            await _store.RevokeAllForUserAsync(userId);
            return await CreateKeyAsync(userId);
        }

        private string GenerateKey()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return "galaxai_" + Convert.ToBase64String(bytes).Replace("=","").Replace("+","-").Replace("/","_");
        }

        private string HashKey(string key)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
            return Convert.ToHexString(hash);
        }

        public async Task<string> ValidateKeyAsync(string incomingKey)
        {
            if (string.IsNullOrEmpty(incomingKey)) return null;
            var hash = HashKey(incomingKey);
            var entity = await _store.GetApiKeyByHashAsync(hash);
            if (entity == null || entity.Revoked) return null;
            return entity.UserId;
        }
    }
}
