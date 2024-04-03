using Microsoft.AspNetCore.Mvc;

namespace eLearningApi.Controllers;

public class ErrorHandlingController : ControllerBase
{
    protected IActionResult InternalError(string? message = null)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, message ?? "There was an internal error.");
    }
}
