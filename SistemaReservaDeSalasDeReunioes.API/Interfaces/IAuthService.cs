using SistemaReservaDeSalasDeReunioes.API.DTOs;
using SistemaReservaDeSalasDeReunioes.API.Models;

namespace SistemaReservaDeSalasDeReunioes.API.Interfaces
{
 public interface IAuthService
 {
 Task<(bool ok, string? error)> RegisterAsync(RegisterRequestDto dto);
 Task<(bool ok, string? error, LoginResponseDto? token)> LoginAsync(LoginRequestDto dto);
 }
}