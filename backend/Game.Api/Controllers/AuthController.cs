using Game.Auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace Game.Api.Controllers
{
    [ApiController]
    [Route("auth/keys")]
    public class AuthController : ControllerBase
    {
        private readonly ApiKeyService _apiKeyService;
        private readonly string _adminKey;

        public AuthController(ApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
            _adminKey = Environment.GetEnvironmentVariable("ADMIN_API_KEY") ?? string.Empty;
        }

        private bool IsAuthorized()
        {
            if (string.IsNullOrEmpty(_adminKey)) return false;
            if (!Request.Headers.TryGetValue("Authorization", out var provided)) return false;
            var header = provided.ToString();
            if (!header.StartsWith("Bearer ")) return false;
            var token = header.Substring("Bearer ".Length).Trim();
            return token == _adminKey;
        }

        [HttpPost]
        public async Task<IActionResult> CreateKey([FromBody] CreateKeyRequest req)
        {
            if (!IsAuthorized()) return Unauthorized();
            if (string.IsNullOrWhiteSpace(req.UserId)) return BadRequest("user_id required");
            var res = await _apiKeyService.CreateKeyAsync(req.UserId, req.Note);
            return CreatedAtAction(nameof(GetKeys), new { userId = req.UserId }, new { key_id = res.KeyId, raw_key = res.RawKey, created_at = System.DateTime.UtcNow });
        }

        [HttpGet]
        public async Task<IActionResult> GetKeys([FromQuery] string userId)
        {
            if (!IsAuthorized()) return Unauthorized();
            if (string.IsNullOrWhiteSpace(userId)) return BadRequest("user_id query required");
            var keys = await _apiKeyService.ListKeysAsync(userId);
            var list = keys.Select(k => new { key_id = k.RowKey, created_at = k.GetDateTime("CreatedAt"), revoked = k.GetBoolean("Revoked") });
            return Ok(list);
        }

        [HttpDelete("{keyId}")]
        public async Task<IActionResult> RevokeKey(string keyId, [FromQuery] string userId)
        {
            if (!IsAuthorized()) return Unauthorized();
            if (string.IsNullOrWhiteSpace(userId)) return BadRequest("user_id query required");
            await _apiKeyService.RevokeKeyAsync(userId, keyId);
            return NoContent();
        }
    }

    public record CreateKeyRequest(string UserId, string? Note);
}
