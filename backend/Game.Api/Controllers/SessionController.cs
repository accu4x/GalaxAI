using Game.NpcSkill.Dialog;
using Microsoft.AspNetCore.Mvc;

namespace Game.Api.Controllers;

[ApiController]
[Route("session")]
public class SessionController : ControllerBase
{
    private readonly DialogRunner _runner;

    public SessionController(DialogRunner runner)
    {
        _runner = runner;
    }

    public class StartRequest
    {
        public string NpcId { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public Dictionary<string, object>? Snapshot { get; set; }
    }

    [HttpPost("start")]
    public IActionResult Start([FromBody] StartRequest req)
    {
        var snapshot = new PlayerSnapshot();
        if (req.Snapshot != null)
        {
            foreach(var kv in req.Snapshot)
            {
                snapshot[kv.Key] = kv.Value!;
            }
        }
        var (session, formatted) = _runner.StartSession(req.NpcId, req.OwnerId, snapshot);
        return Ok(new { sessionId = session.Id, reply = formatted });
    }

    public class MessageRequest
    {
        public string SessionId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, object>? Snapshot { get; set; }
    }

    [HttpPost("message")]
    public IActionResult Message([FromBody] MessageRequest req)
    {
        var snapshot = new PlayerSnapshot();
        if (req.Snapshot != null)
        {
            foreach(var kv in req.Snapshot)
            {
                snapshot[kv.Key] = kv.Value!;
            }
        }
        var (formatted, raw) = _runner.SendMessage(req.SessionId, req.Message, snapshot);
        return Ok(new { reply = formatted, raw = raw });
    }
}
