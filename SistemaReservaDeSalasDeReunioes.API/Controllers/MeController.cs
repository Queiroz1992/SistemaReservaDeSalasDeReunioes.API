using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SistemaReservaDeSalasDeReunioes.API.Controllers
{
 [ApiController]
 [Route("api/[controller]")]
 public class MeController : ControllerBase
 {
 [HttpGet]
 [Authorize]
 public IActionResult Get()
 {
 var claims = User.Claims.Select(c => new { c.Type, c.Value });
 return Ok(claims);
 }
 }
}