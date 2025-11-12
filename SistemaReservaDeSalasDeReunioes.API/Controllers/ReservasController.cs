using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaReservaDeSalasDeReunioes.API.DTOs;
using SistemaReservaDeSalasDeReunioes.API.Filters;
using SistemaReservaDeSalasDeReunioes.API.Interfaces;

namespace SistemaReservaDeSalasDeReunioes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservasController : ControllerBase
{
    private readonly IReservaService _service;
    private readonly IReservaMapper _mapper;

    public ReservasController(IReservaService service, IReservaMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservaDto>>> GetAll()
    {
        var reservas = await _service.GetAllAsync();
        var dtos = reservas.Select(_mapper.ToDto);
        return Ok(dtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReservaDto>> GetById(int id)
    {
        var reserva = await _service.GetByIdAsync(id);
        if (reserva == null) return NotFound();
        return Ok(_mapper.ToDto(reserva));
    }

    [HttpPost]
    public async Task<ActionResult<ReservaDto>> Create([FromBody] ReservaCreateDto createDto)
    {
        var reserva = _mapper.ToEntity(createDto);
        var (ok, error) = await _service.CreateAsync(reserva);
        if (!ok) return BadRequest(new { error });
        return CreatedAtAction(nameof(GetById), new { id = reserva.Id }, _mapper.ToDto(reserva));
    }

    [HttpPut("{id:int}")]
    [ValidateRouteId]
    public async Task<IActionResult> Edit(int id, [FromBody] ReservaUpdateDto updateDto)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing == null) return NotFound();
        _mapper.MapUpdate(updateDto, existing);
        var (ok, error) = await _service.UpdateAsync(existing);
        if (!ok) return BadRequest(new { error });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
