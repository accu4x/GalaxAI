using Game.NpcSkill;
using Microsoft.AspNetCore.Mvc;

namespace Game.Api.Controllers;

[ApiController]
[Route("npcs")]
public class NpcController : ControllerBase
{
    private readonly NpcService _npcService;

    public NpcController(NpcService npcService)
    {
        _npcService = npcService;
    }

    [HttpGet]
    public IActionResult List()
    {
        var ids = _npcService.ListNpcIds();
        return Ok(ids);
    }

    [HttpGet("{id}/md")]
    public IActionResult GetMd(string id)
    {
        var md = _npcService.GetRawMarkdown(id);
        if (string.IsNullOrEmpty(md)) return NotFound();
        return Content(md, "text/markdown");
    }

    [HttpPost("refresh")]
    public IActionResult Refresh()
    {
        // For now we reload on construction; a full refresh would trigger the skill to re-sync from blob/table
        return Ok(new { status = "refresh not implemented in demo" });
    }
}
