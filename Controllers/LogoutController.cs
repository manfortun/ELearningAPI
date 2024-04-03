using eLearningApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace eLearningApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LogoutController : ControllerBase
{
    private readonly TokenManager _tokenManager;

    public LogoutController(TokenManager tokenManager)
    {
        _tokenManager = tokenManager;
    }

    [HttpPost]
    public IActionResult Logout()
    {
        string? token = TokenService.GetAuthToken(HttpContext);

        if (string.IsNullOrEmpty(token))
        {
            return BadRequest();
        }

        if (_tokenManager.IsTokenActive(token))
        {
            return Ok();
        }

        _tokenManager.DeactivateToken(token);
        return Ok();
    }
}
