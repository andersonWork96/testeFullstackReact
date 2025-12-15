using HrManager.Application.Abstractions.Services;
using HrManager.Application.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace HrManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogWarning("Falha de login para {Email}", request.Email);
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Value);
    }
}