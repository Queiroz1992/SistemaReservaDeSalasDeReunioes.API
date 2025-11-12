using SistemaReservaDeSalasDeReunioes.API.Models;

namespace SistemaReservaDeSalasDeReunioes.API.Interfaces;

public interface IReservaRepository
{
    Task<List<Reserva>> GetAllAsync();
    Task<Reserva?> GetByIdAsync(int id);
    Task AddAsync(Reserva reserva);
    Task UpdateAsync(Reserva reserva);
    Task DeleteAsync(Reserva reserva);
    Task<bool> ExistsConflictAsync(int salaId, DateTime inicio, DateTime fim, int? excludeId = null);
}
