using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SistemaReservaDeSalasDeReunioes.API.Data;
using SistemaReservaDeSalasDeReunioes.API.DTOs;
using SistemaReservaDeSalasDeReunioes.API.Interfaces;
using SistemaReservaDeSalasDeReunioes.API.Models;

namespace SistemaReservaDeSalasDeReunioes.API.Services
{
 public class JwtOptions
 {
 public string Issuer { get; set; } = "ReservaSalasApi";
 public string Audience { get; set; } = "ReservaSalasApiAudience";
 public string SigningKey { get; set; } = "CHANGE_THIS_SUPER_SECRET_KEY_MIN_32_CHARS"; // dev only
 public int ExpirationMinutes { get; set; } =60;
 }

 public class AuthService : IAuthService
 {
 private readonly AppDbContext _ctx;
 private readonly JwtOptions _jwtOptions;

 public AuthService(AppDbContext ctx, IOptions<JwtOptions> jwtOptions)
 {
 _ctx = ctx;
 _jwtOptions = jwtOptions.Value;
 }

 public async Task<(bool ok, string? error)> RegisterAsync(RegisterRequestDto dto)
 {
 if (await _ctx.Usuarios.AnyAsync(u => u.Email == dto.Email))
 return (false, "Email já registrado");
 var hash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
 var usuario = new Usuario { Nome = dto.Nome, Email = dto.Email, SenhaHash = hash };
 _ctx.Usuarios.Add(usuario);
 await _ctx.SaveChangesAsync();
 return (true, null);
 }

 public async Task<(bool ok, string? error, LoginResponseDto? token)> LoginAsync(LoginRequestDto dto)
 {
 var usuario = await _ctx.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
 if (usuario == null) return (false, "Credenciais inválidas", null);
 if (!BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash)) return (false, "Credenciais inválidas", null);
 var now = DateTime.UtcNow;
 var expires = now.AddMinutes(_jwtOptions.ExpirationMinutes);
 var claims = new List<Claim>
 {
 new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
 new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
 new Claim("nome", usuario.Nome)
 };
 var keyBytes = Encoding.UTF8.GetBytes(_jwtOptions.SigningKey);
 var key = new SymmetricSecurityKey(keyBytes);
 var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
 var jwt = new JwtSecurityToken(
 issuer: _jwtOptions.Issuer,
 audience: _jwtOptions.Audience,
 claims: claims,
 notBefore: now,
 expires: expires,
 signingCredentials: creds);
 var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);
 return (true, null, new LoginResponseDto(tokenString, "Bearer", expires, usuario.Nome, usuario.Email));
 }
 }
}