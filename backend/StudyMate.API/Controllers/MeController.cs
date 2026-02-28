using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StudyMate.API.Controllers;

[ApiController]
[Route("me")]
public class MeController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public IActionResult GetMe()
    {
        var userId = User.FindFirstValue("uid");

        return Ok(new
        {
            message = "Authorized",
            userId
        });
    }
}