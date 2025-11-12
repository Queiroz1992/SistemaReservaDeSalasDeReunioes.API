# Sistema de Reserva de Salas de Reuni�es (API .NET8)

API REST para gerenciar reservas de salas com valida��o de conflitos, autentica��o JWT e documenta��o via Swagger. Persist�ncia em SQL Server.

## Tecnologias usadas
- .NET8 (ASP.NET Core Web API)
- Entity Framework Core8
 - Provider: SQL Server (`Microsoft.EntityFrameworkCore.SqlServer`)
 - Migra��es (`Microsoft.EntityFrameworkCore.Tools`)
- Autentica��o: JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- Hash de senha: `BCrypt.Net-Next`
- Swagger/OpenAPI: `Swashbuckle.AspNetCore`
- Testes: `xUnit`, `Microsoft.NET.Test.Sdk`

## Pr�?requisitos
- SDK .NET8
- SQL Server acess�vel (inst�ncia configurada em `appsettings.json`)
- (Opcional) Ferramenta de migra��es: `dotnet tool install -g dotnet-ef`

## Configura��o
Arquivo `SistemaReservaDeSalasDeReunioes.API/appsettings.json`:
- ConnectionStrings: `DefaultConnection` (atualmente: `Server=colocar a string de conec a ser usada;Database=ReservasDb;Trusted_Connection=True;...`)
- JWT (`Jwt`): Issuer, Audience, SigningKey (troque por chave forte em produ��o) e ExpirationMinutes.

## Como executar
Via CLI:
1) Restaurar depend�ncias
 - `dotnet restore`
2) Criar/atualizar banco (migra��es j� inclusas no projeto)
 - `dotnet ef database update --project SistemaReservaDeSalasDeReunioes.API`
3) Executar API
 - `dotnet run --project SistemaReservaDeSalasDeReunioes.API`
4) Abrir Swagger: `https://localhost:<porta>/swagger`

Observa��o: No startup � chamado `ctx.Database.Migrate()` para aplicar migra��es automaticamente.

## Autentica��o (JWT)
- Login: `POST /api/auth/login` com `{ "email": "admin@local", "senha": "admin123" }`
- Resposta cont�m `accessToken` (Bearer). Use o bot�o Authorize no Swagger e informe `Bearer <token>`.
- Registro: `POST /api/auth/register` (an�nimo).
- Endpoint para ver claims: `GET /api/me` (protegido).

Claims emitidas: `sub` (id), `email`, `nome`.

## Endpoints principais
- Auth (an�nimo)
 - `POST /api/auth/register`
 - `POST /api/auth/login`
- Me (protegido)
 - `GET /api/me`
- Salas (somente leitura)
 - `GET /api/salas`
 - `GET /api/salas/{id}`
 - `GET /api/salas/count`
- Reservas (protegido por JWT)
 - `GET /api/Reservas`
 - `GET /api/Reservas/{id}`
 - `POST /api/Reservas`
 - `PUT /api/Reservas/{id}`
 - `DELETE /api/Reservas/{id}`

### Regras de neg�cio (Reservas)
- `Fim` deve ser maior que `In�cio`.
- N�o pode haver sobreposi��o de hor�rio para a mesma sala.
- Se `Cafe` = true, `QuantidadeCafe` deve ser >0.

## Seed de dados
Na primeira execu��o (ap�s migra��es):
- Salas: Pr�dio A/Sala101 (10), Pr�dio A/Sala102 (8), Pr�dio B/Sala201 (12)
- Reservas:3 exemplos
- Usu�rio admin: `admin@local` / `admin123`

## Testes
- Executar: `dotnet test`
- Cobrem valida��es de reserva (conflito, hor�rios, caf�, delete).

## Swagger
- Habilitado no ambiente Development.
- Suporta autentica��o Bearer (bot�o Authorize).

## Dicas e problemas comuns
-401/invalid_token: gere novo login ap�s alterar Issuer/Audience/SigningKey; confirme prefixo `Bearer`.
- CORS: n�o habilitado. Adicione se um frontend externo for consumir a API.
- Produ��o: use migra��es (j� configurado), connection string segura e chave JWT forte.
