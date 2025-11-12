using SistemaReservaDeSalasDeReunioes.API.Models;

namespace SistemaReservaDeSalasDeReunioes.API.Interfaces;

public interface IReservaBusinessValidator
{
    Task<(bool Ok, string? Error)> ValidateCreateAsync(Reserva reserva);
    Task<(bool Ok, string? Error)> ValidateUpdateAsync(Reserva reserva);
}
