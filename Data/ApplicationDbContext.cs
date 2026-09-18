using LocadoraApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LocadoraApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Fabricante> Fabricantes { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Aluguel> Alugueis { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fabricante -> Veiculo (1:N) - Restrict: não pode apagar um fabricante com veículos cadastrados
            modelBuilder.Entity<Veiculo>()
                .HasOne(v => v.Fabricante)
                .WithMany(f => f.Veiculos)
                .HasForeignKey(v => v.FabricanteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cliente -> Aluguel (1:N) - Restrict: não pode apagar cliente com aluguel vinculado
            modelBuilder.Entity<Aluguel>()
                .HasOne(a => a.Cliente)
                .WithMany(c => c.Alugueis)
                .HasForeignKey(a => a.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Veiculo -> Aluguel (1:N) - Restrict: não pode apagar veículo com aluguel vinculado
            modelBuilder.Entity<Aluguel>()
                .HasOne(a => a.Veiculo)
                .WithMany(v => v.Alugueis)
                .HasForeignKey(a => a.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Aluguel -> Pagamento (1:N) - Cascade: ao apagar um aluguel, apaga os pagamentos dele
            modelBuilder.Entity<Pagamento>()
                .HasOne(p => p.Aluguel)
                .WithMany(a => a.Pagamentos)
                .HasForeignKey(p => p.AluguelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice único de CPF
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Cpf)
                .IsUnique();
        }
    }
}