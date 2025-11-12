using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using SistemaReservaDeSalasDeReunioes.API.Data;
using SistemaReservaDeSalasDeReunioes.API.Interfaces;
using SistemaReservaDeSalasDeReunioes.API.Models;
using SistemaReservaDeSalasDeReunioes.API.Repositories;
using SistemaReservaDeSalasDeReunioes.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// EF Core SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
 ?? "Server=G4F-58WTPP3\\SQLSERVER2022;Database=ReservasDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
builder.Services.AddDbContext<AppDbContext>(options =>
 options.UseSqlServer(connectionString));

// Bind JwtOptions once and reuse the same values for signing and validation
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey) || Encoding.UTF8.GetByteCount(jwtOptions.SigningKey) <32)
{
 // Guarantee a minimum size and avoid empty key
 jwtOptions.SigningKey = new JwtOptions().SigningKey;
}
// Make options injectable
builder.Services.Configure<JwtOptions>(opt =>
{
 opt.Issuer = jwtOptions.Issuer;
 opt.Audience = jwtOptions.Audience;
 opt.SigningKey = jwtOptions.SigningKey;
 opt.ExpirationMinutes = jwtOptions.ExpirationMinutes;
});

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey));

builder.Services.AddAuthentication(options =>
{
 options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
 options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
 options.TokenValidationParameters = new TokenValidationParameters
 {
 ValidateIssuer = true,
 ValidateAudience = true,
 ValidateIssuerSigningKey = true,
 ValidIssuer = jwtOptions.Issuer,
 ValidAudience = jwtOptions.Audience,
 IssuerSigningKey = signingKey,
 ClockSkew = TimeSpan.Zero
 };
 // Expor mensagens de diagnóstico para facilitar troubleshooting
 options.Events = new JwtBearerEvents
 {
 OnAuthenticationFailed = ctx =>
 {
 ctx.Response.Headers["x-auth-error"] = ctx.Exception.GetType().Name + ": " + ctx.Exception.Message;
 return Task.CompletedTask;
 }
 };
});

// DI registrations
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<ISalaRepository, SalaRepository>();
builder.Services.AddScoped<IReservaBusinessValidator, ReservaBusinessValidator>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IReservaMapper, ReservaMapper>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Swagger for API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
 c.SwaggerDoc("v1", new OpenApiInfo { Title = "SistemaReservaDeSalasDeReunioes API", Version = "v1" });
 var securityScheme = new OpenApiSecurityScheme
 {
 Name = "Authorization",
 Description = "Insira o token JWT no formato: Bearer {token}",
 In = ParameterLocation.Header,
 Type = SecuritySchemeType.Http,
 Scheme = "bearer",
 BearerFormat = "JWT",
 Reference = new OpenApiReference
 {
 Type = ReferenceType.SecurityScheme,
 Id = "Bearer"
 }
 };
 c.AddSecurityDefinition("Bearer", securityScheme);
 c.AddSecurityRequirement(new OpenApiSecurityRequirement
 {
 { securityScheme, new string[] {} }
 });
});

var app = builder.Build();

// Log básico para diagnosticar configuração (sem vazar segredo)
app.Logger.LogInformation("JWT Issuer: {iss} | Audience: {aud} | KeyLen: {len}", jwtOptions.Issuer, jwtOptions.Audience, jwtOptions.SigningKey.Length);

// Apply database schema and seed data
using (var scope = app.Services.CreateScope())
{
 var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
 // Use migrations em produção
 ctx.Database.Migrate();

 if (!ctx.Salas.Any())
 {
 ctx.Salas.AddRange(new List<Sala>
 {
 new Sala { Local = "Prédio A", Nome = "Sala101", Capacidade =10 },
 new Sala { Local = "Prédio A", Nome = "Sala102", Capacidade =8 },
 new Sala { Local = "Prédio B", Nome = "Sala201", Capacidade =12 }
 });
 ctx.SaveChanges();
 }

 if (!ctx.Reservas.Any())
 {
 var start = DateTime.Today.AddHours(9);
 var sala101Id = ctx.Salas.First(s => s.Nome == "Sala101").Id;
 var sala102Id = ctx.Salas.First(s => s.Nome == "Sala102").Id;
 var sala201Id = ctx.Salas.First(s => s.Nome == "Sala201").Id;
 ctx.Reservas.AddRange(new List<Reserva>
 {
 new Reserva { SalaId = sala101Id, Responsavel = "Alice", Inicio = start, Fim = start.AddHours(1), Cafe = false },
 new Reserva { SalaId = sala102Id, Responsavel = "Bob", Inicio = start.AddHours(2), Fim = start.AddHours(3), Cafe = true, QuantidadeCafe =5, DescricaoCafe = "Café coado" },
 new Reserva { SalaId = sala201Id, Responsavel = "Carol", Inicio = start.AddHours(4), Fim = start.AddHours(5), Cafe = false }
 });
 ctx.SaveChanges();
 }

 if (!ctx.Usuarios.Any())
 {
 var hash = BCrypt.Net.BCrypt.HashPassword("admin123");
 ctx.Usuarios.Add(new Usuario { Nome = "Admin", Email = "admin@local", SenhaHash = hash });
 ctx.SaveChanges();
 }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
 app.UseSwagger();
 app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
