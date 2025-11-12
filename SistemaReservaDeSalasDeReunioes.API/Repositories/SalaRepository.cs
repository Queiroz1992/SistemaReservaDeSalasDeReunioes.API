using Microsoft.EntityFrameworkCore;
using SistemaReservaDeSalasDeReunioes.API.Data;
using SistemaReservaDeSalasDeReunioes.API.Interfaces;
using SistemaReservaDeSalasDeReunioes.API.Models;

namespace SistemaReservaDeSalasDeReunioes.API.Repositories;

public class SalaRepository : ISalaRepository
{
    private readonly AppDbContext _ctx;
    public SalaRepository(AppDbContext ctx) => _ctx = ctx;
    public Task<bool> ExistsAsync(int id) => _ctx.Salas.AnyAsync(s => s.Id == id);
    public async Task<IEnumerable<Sala>> GetAllAsync() => await _ctx.Salas.AsNoTracking().ToListAsync();
    public Task<Sala?> GetByIdAsync(int id) => _ctx.Salas.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
}
