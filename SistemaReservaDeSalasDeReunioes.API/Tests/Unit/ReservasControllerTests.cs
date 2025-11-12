using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SistemaReservaDeSalasDeReunioes.API.Controllers;
using SistemaReservaDeSalasDeReunioes.API.Data;
using SistemaReservaDeSalasDeReunioes.API.DTOs;
using SistemaReservaDeSalasDeReunioes.API.Interfaces;
using SistemaReservaDeSalasDeReunioes.API.Models;
using SistemaReservaDeSalasDeReunioes.API.Repositories;
using SistemaReservaDeSalasDeReunioes.API.Services;
using Xunit;

namespace SistemaReservaDeSalasDeReunioes.API.Tests.Unit;

public class ReservasControllerTests
{
    private readonly AppDbContext _ctx;
    private readonly IReservaRepository _repo;
    private readonly IReservaBusinessValidator _validator;
    private readonly IReservaService _service;
    private readonly IReservaMapper _mapper;
    private readonly ReservasController _controller;

    public ReservasControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _ctx = new AppDbContext(options);
        _repo = new ReservaRepository(_ctx);
        var salaRepo = new SalaRepository(_ctx);
        _validator = new ReservaBusinessValidator(_repo, salaRepo);
        _service = new ReservaService(_repo, _validator);
        _mapper = new ReservaMapper();
        _controller = new ReservasController(_service, _mapper);

        _ctx.Salas.Add(new Sala { Id = 1, Local = "Prédio A", Nome = "Sala 101", Capacidade = 10 });
        _ctx.SaveChanges();
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_AndEmptyListInitially()
    {
        var result = await _controller.GetAll();
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<ReservaDto>>(ok.Value);
        Assert.Empty(dtos);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenFimAntesDoInicio()
    {
        var inicio = DateTime.Today.AddHours(10);
        var fim = DateTime.Today.AddHours(9); // inválido
        var dto = new ReservaCreateDto
        {
            SalaId = 1,
            Responsavel = "Resp",
            Inicio = inicio,
            Fim = fim,
            Cafe = false,
            QuantidadeCafe = null,
            DescricaoCafe = null
        };
        var result = await _controller.Create(dto);
        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("Horário final", bad.Value!.ToString());
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenValido()
    {
        var dto = new ReservaCreateDto
        {
            SalaId = 1,
            Responsavel = "Resp",
            Inicio = DateTime.Today.AddHours(11),
            Fim = DateTime.Today.AddHours(12),
            Cafe = true,
            QuantidadeCafe = 3,
            DescricaoCafe = "Café"
        };
        var result = await _controller.Create(dto);
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var reservaDto = Assert.IsType<ReservaDto>(created.Value);
        Assert.Equal(dto.SalaId, reservaDto.SalaId);
        Assert.Equal(dto.Responsavel, reservaDto.Responsavel);
        Assert.True(reservaDto.Id > 0);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenExiste()
    {
        var dto = new ReservaCreateDto
        {
            SalaId = 1,
            Responsavel = "Resp2",
            Inicio = DateTime.Today.AddHours(13),
            Fim = DateTime.Today.AddHours(14),
            Cafe = false,
            QuantidadeCafe = null,
            DescricaoCafe = null
        };
        var createResult = await _controller.Create(dto);
        var created = Assert.IsType<CreatedAtActionResult>(createResult.Result);
        var reservaDto = Assert.IsType<ReservaDto>(created.Value);

        var getResult = await _controller.GetById(reservaDto.Id);
        var ok = Assert.IsType<OkObjectResult>(getResult.Result);
        var fetched = Assert.IsType<ReservaDto>(ok.Value);
        Assert.Equal(reservaDto.Id, fetched.Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenInexistente()
    {
        var result = await _controller.GetById(999); // não criado
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenExiste()
    {
        var dto = new ReservaCreateDto
        {
            SalaId = 1,
            Responsavel = "Resp3",
            Inicio = DateTime.Today.AddHours(15),
            Fim = DateTime.Today.AddHours(16),
            Cafe = false,
            QuantidadeCafe = null,
            DescricaoCafe = null
        };
        var createResult = await _controller.Create(dto);
        var created = Assert.IsType<CreatedAtActionResult>(createResult.Result);
        var reservaDto = Assert.IsType<ReservaDto>(created.Value);

        var deleteResult = await _controller.Delete(reservaDto.Id);
        Assert.IsType<NoContentResult>(deleteResult);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenInexistente()
    {
        var deleteResult = await _controller.Delete(777);
        Assert.IsType<NotFoundResult>(deleteResult);
    }
}
