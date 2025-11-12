using SistemaReservaDeSalasDeReunioes.API.Models;

namespace SistemaReservaDeSalasDeReunioes.API.Interfaces;

public interface ISalaRepository
{
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<Sala>> GetAllAsync();
    Task<Sala?> GetByIdAsync(int id);
}
