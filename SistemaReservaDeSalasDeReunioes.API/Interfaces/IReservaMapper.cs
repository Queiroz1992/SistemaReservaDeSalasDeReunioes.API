using SistemaReservaDeSalasDeReunioes.API.DTOs;
using SistemaReservaDeSalasDeReunioes.API.Models;

namespace SistemaReservaDeSalasDeReunioes.API.Interfaces;

public interface IReservaMapper
{
    Reserva ToEntity(ReservaCreateDto dto);
    void MapUpdate(ReservaUpdateDto dto, Reserva entity);
    ReservaDto ToDto(Reserva entity);
}
