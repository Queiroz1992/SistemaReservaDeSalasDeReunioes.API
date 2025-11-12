using SistemaReservaDeSalasDeReunioes.API.DTOs;
using SistemaReservaDeSalasDeReunioes.API.Interfaces;
using SistemaReservaDeSalasDeReunioes.API.Models;

namespace SistemaReservaDeSalasDeReunioes.API.Services;

public class ReservaMapper : IReservaMapper
{
    public Reserva ToEntity(ReservaCreateDto dto) => new()
    {
        SalaId = dto.SalaId,
        Responsavel = dto.Responsavel,
        Inicio = dto.Inicio,
        Fim = dto.Fim,
        Cafe = dto.Cafe,
        QuantidadeCafe = dto.QuantidadeCafe,
        DescricaoCafe = dto.DescricaoCafe
    };

    public void MapUpdate(ReservaUpdateDto dto, Reserva entity)
    {
        entity.SalaId = dto.SalaId;
        entity.Responsavel = dto.Responsavel;
        entity.Inicio = dto.Inicio;
        entity.Fim = dto.Fim;
        entity.Cafe = dto.Cafe;
        entity.QuantidadeCafe = dto.QuantidadeCafe;
        entity.DescricaoCafe = dto.DescricaoCafe;
    }

    public ReservaDto ToDto(Reserva entity) => new ReservaDto
    {
        Id = entity.Id,
        SalaId = entity.SalaId,
        SalaNome = entity.Sala?.Nome ?? string.Empty,
        SalaLocal = entity.Sala?.Local ?? string.Empty,
        Responsavel = entity.Responsavel,
        Inicio = entity.Inicio,
        Fim = entity.Fim,
        Cafe = entity.Cafe,
        QuantidadeCafe = entity.QuantidadeCafe,
        DescricaoCafe = entity.DescricaoCafe
    };
}
