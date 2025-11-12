using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaReservaDeSalasDeReunioes.API.DTOs;
using SistemaReservaDeSalasDeReunioes.API.Interfaces;

namespace SistemaReservaDeSalasDeReunioes.API.Controllers
{
 [ApiController]
 [Route("api/[controller]")]
 public class AuthController : ControllerBase
 {
 private readonly IAuthService _authService;
 public AuthController(IAuthService authService)
 {
 _authService = authService;
 }

 [HttpPost("register")]
 [AllowAnonymous]
 public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
 {
 var (ok, error) = await _authService.RegisterAsync(dto);
 if (!ok) return BadRequest(new { error });
 return NoContent();
 }

 [HttpPost("login")]
 [AllowAnonymous]
 public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
 {
 var (ok, error, token) = await _authService.LoginAsync(dto);
 if (!ok) return Unauthorized(new { error });
 return Ok(token);
 }
 }
}