using Microsoft.EntityFrameworkCore;
using SistemaReservaDeSalasDeReunioes.API.Data;
using SistemaReservaDeSalasDeReunioes.API.Interfaces;
using SistemaReservaDeSalasDeReunioes.API.Models;
using SistemaReservaDeSalasDeReunioes.API.Repositories;
using SistemaReservaDeSalasDeReunioes.API.Services;
using Xunit;

namespace SistemaReservaDeSalasDeReunioes.API.Tests.Unit;

public class ReservaServiceTests
{
    private readonly IReservaService _service;
    private readonly AppDbContext _ctx;

    public ReservaServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _ctx = new AppDbContext(options);
        var repo = new ReservaRepository(_ctx);
        var salaRepo = new SalaRepository(_ctx);
        var validator = new ReservaBusinessValidator(repo, salaRepo);
        _service = new ReservaService(repo, validator);

        _ctx.Salas.Add(new Sala { Id = 1, Local = "Prédio A", Nome = "Sala 101", Capacidade = 10 });
        _ctx.SaveChanges();
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenFimAntesOuIgualInicio()
    {
        var reserva = new Reserva
        {
            SalaId = 1,
            Responsavel = "Teste",
            Inicio = DateTime.Today.AddHours(10),
            Fim = DateTime.Today.AddHours(10), // igual
            Cafe = false
        };
        var (ok, error) = await _service.CreateAsync(reserva);
        Assert.False(ok);
        Assert.Equal("Horário final deve ser maior que o inicial.", error);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenConflitoHorario()
    {
        var r1 = new Reserva
        {
            SalaId = 1,
            Responsavel = "A",
            Inicio = DateTime.Today.AddHours(9),
            Fim = DateTime.Today.AddHours(10)
        };
        var r2 = new Reserva
        {
            SalaId = 1,
            Responsavel = "B",
            Inicio = DateTime.Today.AddHours(9).AddMinutes(30),
            Fim = DateTime.Today.AddHours(10).AddMinutes(30)
        };
        var (ok1, _) = await _service.CreateAsync(r1);
        var (ok2, error2) = await _service.CreateAsync(r2);
        Assert.True(ok1);
        Assert.False(ok2);
        Assert.Equal("Já existe uma reserva nesse período para a sala escolhida.", error2);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenCafeQuantidadeInvalida()
    {
        var reserva = new Reserva
        {
            SalaId = 1,
            Responsavel = "Teste",
            Inicio = DateTime.Today.AddHours(8),
            Fim = DateTime.Today.AddHours(9),
            Cafe = true,
            QuantidadeCafe = -1
        };
        var (ok, error) = await _service.CreateAsync(reserva);
        Assert.False(ok);
        Assert.Equal("Informe a quantidade de café.", error);
    }

    [Fact]
    public async Task CreateAsync_ShouldSucceed_WhenDadosValidos()
    {
        var reserva = new Reserva
        {
            SalaId = 1,
            Responsavel = "Responsável",
            Inicio = DateTime.Today.AddHours(11),
            Fim = DateTime.Today.AddHours(12),
            Cafe = true,
            QuantidadeCafe = 5,
            DescricaoCafe = "Café e água"
        };
        var (ok, error) = await _service.CreateAsync(reserva);
        Assert.True(ok);
        Assert.Null(error);
        Assert.True(reserva.Id > 0);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenConflito()
    {
        var r1 = new Reserva
        {
            SalaId = 1,
            Responsavel = "A",
            Inicio = DateTime.Today.AddHours(13),
            Fim = DateTime.Today.AddHours(14)
        };
        var r2 = new Reserva
        {
            SalaId = 1,
            Responsavel = "B",
            Inicio = DateTime.Today.AddHours(15),
            Fim = DateTime.Today.AddHours(16)
        };
        await _service.CreateAsync(r1);
        await _service.CreateAsync(r2);
        // tentar mudar r2 para conflitar com r1
        r2.Inicio = DateTime.Today.AddHours(13).AddMinutes(30);
        r2.Fim = DateTime.Today.AddHours(14).AddMinutes(30);
        var (ok, error) = await _service.UpdateAsync(r2);
        Assert.False(ok);
        Assert.Equal("Já existe uma reserva nesse período para a sala escolhida.", error);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
    {
        var result = await _service.DeleteAsync(999);
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenExists()
    {
        var reserva = new Reserva
        {
            SalaId = 1,
            Responsavel = "Del",
            Inicio = DateTime.Today.AddHours(17),
            Fim = DateTime.Today.AddHours(18)
        };
        var (ok, _) = await _service.CreateAsync(reserva);
        Assert.True(ok);
        var deleted = await _service.DeleteAsync(reserva.Id);
        Assert.True(deleted);
    }
}
