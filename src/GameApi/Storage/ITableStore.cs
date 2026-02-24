namespace GameApi.Storage
{
    public interface ITableStore
    {
        Task<PlayerEntity> GetPlayerAsync(string playerId);
        Task UpsertPlayerAsync(PlayerEntity player);
        Task InsertApiKeyAsync(ApiKeyEntity key);
        Task<IEnumerable<ApiKeyEntity>> ListApiKeysAsync(string userId);
        Task RevokeApiKeyAsync(string keyId);
        Task RevokeAllForUserAsync(string userId);
        Task<ApiKeyEntity> GetApiKeyByHashAsync(string hash);
    }
}
