using SistemaReservaDeSalasDeReunioes.API.Models;
using SistemaReservaDeSalasDeReunioes.API.Interfaces;

namespace SistemaReservaDeSalasDeReunioes.API.Services;

public class ReservaBusinessValidator : IReservaBusinessValidator
{
    private readonly IReservaRepository _repo;
    private readonly ISalaRepository _salas;
    public ReservaBusinessValidator(IReservaRepository repo, ISalaRepository salas)
    {
        _repo = repo;
        _salas = salas;
    }

    public async Task<(bool Ok, string? Error)> ValidateCreateAsync(Reserva reserva)
        => await ValidateCommonAsync(reserva, null);

    public async Task<(bool Ok, string? Error)> ValidateUpdateAsync(Reserva reserva)
        => await ValidateCommonAsync(reserva, reserva.Id);

    private async Task<(bool Ok, string? Error)> ValidateCommonAsync(Reserva reserva, int? excludeId)
    {
        if (!await _salas.ExistsAsync(reserva.SalaId))
            return (false, "Sala informada não existe.");
        if (reserva.Inicio >= reserva.Fim)
            return (false, "Horário final deve ser maior que o inicial.");
        if (await _repo.ExistsConflictAsync(reserva.SalaId, reserva.Inicio, reserva.Fim, excludeId))
            return (false, "Já existe uma reserva nesse período para a sala escolhida.");
        if (reserva.Cafe && (reserva.QuantidadeCafe is null or < 0))
            return (false, "Informe a quantidade de café.");
        return (true, null);
    }
}

public class ReservaService : IReservaService
{
    private readonly IReservaRepository _repo;
    private readonly IReservaBusinessValidator _validator;

    public ReservaService(IReservaRepository repo, IReservaBusinessValidator validator)
    {
        _repo = repo;
        _validator = validator;
    }

    public Task<List<Reserva>> GetAllAsync() => _repo.GetAllAsync();

    public Task<Reserva?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

    public async Task<(bool Ok, string? Error)> CreateAsync(Reserva reserva)
    {
        var (ok, error) = await _validator.ValidateCreateAsync(reserva);
        if (!ok) return (ok, error);
        await _repo.AddAsync(reserva);
        return (true, null);
    }

    public async Task<(bool Ok, string? Error)> UpdateAsync(Reserva reserva)
    {
        var (ok, error) = await _validator.ValidateUpdateAsync(reserva);
        if (!ok) return (ok, error);
        await _repo.UpdateAsync(reserva);
        return (true, null);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return false;
        await _repo.DeleteAsync(existing);
        return true;
    }
}
