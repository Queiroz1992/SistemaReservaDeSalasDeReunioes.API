using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaReservaDeSalasDeReunioes.API.DTOs;
using SistemaReservaDeSalasDeReunioes.API.Interfaces;
using SistemaReservaDeSalasDeReunioes.API.Models;

namespace SistemaReservaDeSalasDeReunioes.API.Controllers
{
 [ApiController]
 [Route("api/[controller]")]
 [Authorize]
 public class SalasController : ControllerBase
 {
 private readonly ISalaRepository _repo;
 public SalasController(ISalaRepository repo) => _repo = repo;

 [HttpGet]
 [AllowAnonymous]
 public async Task<ActionResult<IEnumerable<SalaDto>>> GetAll()
 {
 var salas = await _repo.GetAllAsync();
 var dtos = salas.Select(s => new SalaDto { Id = s.Id, Local = s.Local, Nome = s.Nome, Capacidade = s.Capacidade });
 return Ok(dtos);
 }

 [HttpGet("{id:int}")]
 [AllowAnonymous]
 public async Task<ActionResult<SalaDto>> GetById(int id)
 {
 var s = await _repo.GetByIdAsync(id);
 if (s == null) return NotFound();
 return Ok(new SalaDto { Id = s.Id, Local = s.Local, Nome = s.Nome, Capacidade = s.Capacidade });
 }

 [HttpGet("count")]
 [AllowAnonymous]
 public async Task<IActionResult> Count()
 {
 var salas = await _repo.GetAllAsync();
 return Ok(new { count = salas.Count() });
 }
 }
}