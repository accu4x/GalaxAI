using Microsoft.AspNetCore.Mvc;

namespace GameApi.Controllers
{
    [ApiController]
    [Route("api/auth/keys")]
    public class AuthController : ControllerBase
    {
        private readonly Services.KeyService _keys;
        public AuthController(Services.KeyService keys) { _keys = keys; }

        [HttpPost]
        public async Task<IActionResult> CreateKey([FromBody] Models.CreateKeyRequest req)
        {
            var key = await _keys.CreateKeyAsync(req.UserId);
            return Ok(new { key = key.PlaintextKey, id = key.Id });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> ListKeys(string userId)
        {
            var keys = await _keys.ListKeysAsync(userId);
            return Ok(keys);
        }

        [HttpDelete("{keyId}")]
        public async Task<IActionResult> RevokeKey(string keyId)
        {
            await _keys.RevokeKeyAsync(keyId);
            return NoContent();
        }

        [HttpPost("rotate/{userId}")]
        public async Task<IActionResult> Rotate(string userId)
        {
            var newKey = await _keys.RotateKeyAsync(userId);
            return Ok(new { key = newKey.PlaintextKey, id = newKey.Id });
        }
    }
}
