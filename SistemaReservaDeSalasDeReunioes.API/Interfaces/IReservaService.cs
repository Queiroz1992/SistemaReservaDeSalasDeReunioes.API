using SistemaReservaDeSalasDeReunioes.API.Models;

namespace SistemaReservaDeSalasDeReunioes.API.Interfaces;

public interface IReservaService
{
    Task<List<Reserva>> GetAllAsync();
    Task<Reserva?> GetByIdAsync(int id);
    Task<(bool Ok, string? Error)> CreateAsync(Reserva reserva);
    Task<(bool Ok, string? Error)> UpdateAsync(Reserva reserva);
    Task<bool> DeleteAsync(int id);
}
