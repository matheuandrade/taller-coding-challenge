using CodingChallenge.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodingChallenge.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("welcome")]
    public IActionResult Welcome(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest(new { message = "Username is required." });
        }

        try
        {
            var message = _userService.GetWelcomeMessage(username);

            if (message != null)
            {
                return Ok(message);
            }
            else
            {
                return NotFound(new { message = "User not found." });
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions and log errors
            return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
        }
    }
}
