using Microsoft.EntityFrameworkCore;
using SistemaReservaDeSalasDeReunioes.API.Models;

namespace SistemaReservaDeSalasDeReunioes.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Sala> Salas => Set<Sala>();
        public DbSet<Reserva> Reservas => Set<Reserva>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Sala)
                .WithMany(s => s.Reservas)
                .HasForeignKey(r => r.SalaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
