
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProcurementApi.Application.Interfaces;
using ProcurementApi.Infrastructure.Identity;

namespace ProcurementApi.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var roles = await userManager.GetRolesAsync(user);
            var token = tokenService.GenerateToken(user.Id, user.Email ?? string.Empty, user.UserName ?? string.Empty, roles);

            return Ok(new LoginResponse(token, user.Email ?? string.Empty, roles));
        }
    }

    public record LoginRequest(string Email, string Password);

    public record LoginResponse(string Token, string Email, IEnumerable<string> Roles);
}