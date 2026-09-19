using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Gestor> Gestores => Set<Gestor>();
    public DbSet<Contacto> Contactos => Set<Contacto>();
    public DbSet<ContactoHistorial> ContactoHistoriales => Set<ContactoHistorial>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Paciente>(e =>
        {
            e.ToTable("Paciente");
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.DocumentoIdentidad).IsUnique();
            e.Property(p => p.Estado).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<Gestor>(e =>
        {
            e.ToTable("Gestor");
            e.HasKey(g => g.Id);
        });

        modelBuilder.Entity<Contacto>(e =>
        {
            e.ToTable("Contacto");
            e.HasKey(c => c.Id);
            e.Property(c => c.Canal).HasConversion<string>().HasMaxLength(20);
            e.Property(c => c.Resultado).HasConversion<string>().HasMaxLength(20);
            e.HasOne(c => c.Paciente).WithMany(p => p.Contactos).HasForeignKey(c => c.PacienteId);
            e.HasOne(c => c.Gestor).WithMany().HasForeignKey(c => c.GestorId);
        });

        modelBuilder.Entity<ContactoHistorial>(e =>
        {
            e.ToTable("ContactoHistorial");
            e.HasKey(h => h.Id);
            e.HasOne(h => h.Contacto).WithMany().HasForeignKey(h => h.ContactoId);
            e.HasOne(h => h.Gestor).WithMany().HasForeignKey(h => h.GestorId);
        });
    }
}
