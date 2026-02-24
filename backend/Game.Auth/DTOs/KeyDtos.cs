using System;

namespace Game.Auth.DTOs
{
    public record KeyInfo(string KeyId, DateTime CreatedAt, bool Revoked, string? Note);
    public record CreateKeyRequest(string UserId, string? Note);
}
