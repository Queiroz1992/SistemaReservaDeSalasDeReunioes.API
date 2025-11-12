using Microsoft.EntityFrameworkCore;
using SistemaReservaDeSalasDeReunioes.API.Data;
using SistemaReservaDeSalasDeReunioes.API.Interfaces;
using SistemaReservaDeSalasDeReunioes.API.Models;

namespace SistemaReservaDeSalasDeReunioes.API.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly AppDbContext _ctx;
    public ReservaRepository(AppDbContext ctx) => _ctx = ctx;

    public Task<List<Reserva>> GetAllAsync() => _ctx.Reservas.Include(r => r.Sala).OrderBy(r => r.Inicio).ToListAsync();

    public Task<Reserva?> GetByIdAsync(int id) => _ctx.Reservas.Include(r => r.Sala).FirstOrDefaultAsync(r => r.Id == id);

    public async Task AddAsync(Reserva reserva)
    {
        _ctx.Reservas.Add(reserva);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(Reserva reserva)
    {
        _ctx.Reservas.Update(reserva);
        await _ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(Reserva reserva)
    {
        _ctx.Reservas.Remove(reserva);
        await _ctx.SaveChangesAsync();
    }

    public Task<bool> ExistsConflictAsync(int salaId, DateTime inicio, DateTime fim, int? excludeId = null)
    {
        return _ctx.Reservas.Where(r => r.SalaId == salaId && r.Id != excludeId)
            .AnyAsync(r => inicio < r.Fim && fim > r.Inicio);
    }
}
