namespace SistemaReservaDeSalasDeReunioes.API.DTOs
{
 public record LoginRequestDto(string Email, string Senha);
 public record LoginResponseDto(string AccessToken, string TokenType, DateTime ExpiresAt, string? Nome, string? Email);
 public record RegisterRequestDto(string Nome, string Email, string Senha);
}