using Api.Data;
using Api.Models;
using Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Api.Tests;

public class ConsultaConCriterioTests
{
    private static AppDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    // Se siembra con un DbContext y se consulta con otro, a propósito: es lo que
    // pasa en la API real (cada request tiene su propio DbContext). Sin esto, el
    // paciente sembrado queda trackeado en el mismo contexto que consulta, y el
    // ORDER BY del SQL no se refleja en la colección al materializarla — el
    // mismo riesgo que AsNoTracking evita en ObtenerConContactosAsync.
    [Fact]
    public async Task ObtenerConContactos_DevuelveContactosOrdenadosPorFechaDescendenteConNombreDelGestor()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var pacienteId = Guid.NewGuid();
        using (var contextoSiembra = new AppDbContext(options))
        {
            contextoSiembra.Database.EnsureCreated();
            var paciente = new Paciente
            {
                Id = pacienteId,
                Nombre = "Maria Fernanda Gomez",
                DocumentoIdentidad = "1032456789",
                Telefono = "3001112233",
                Ciudad = "Bogota",
                FechaInicioTratamiento = new DateOnly(2026, 6, 1),
                Estado = EstadoPaciente.Activo,
                FechaCreacion = DateTime.UtcNow
            };
            var gestor = new Gestor { Id = Guid.NewGuid(), Nombre = "Laura Restrepo" };
            contextoSiembra.Pacientes.Add(paciente);
            contextoSiembra.Gestores.Add(gestor);
            contextoSiembra.Contactos.AddRange(
                new Contacto { Id = Guid.NewGuid(), PacienteId = paciente.Id, GestorId = gestor.Id, Fecha = new DateOnly(2026, 7, 1), Canal = CanalContacto.Llamada, Resultado = ResultadoContacto.NoContesta, FechaRegistro = DateTime.UtcNow },
                new Contacto { Id = Guid.NewGuid(), PacienteId = paciente.Id, GestorId = gestor.Id, Fecha = new DateOnly(2026, 7, 15), Canal = CanalContacto.WhatsApp, Resultado = ResultadoContacto.Contactado, FechaRegistro = DateTime.UtcNow }
            );
            await contextoSiembra.SaveChangesAsync();
        }

        using var contextoConsulta = new AppDbContext(options);
        var service = new PacienteService(contextoConsulta);
        var resultado = await service.ObtenerConContactosAsync(pacienteId);

        Assert.Equal(2, resultado.Contactos.Count);
        Assert.Equal(new DateOnly(2026, 7, 15), resultado.Contactos.First().Fecha);
        Assert.Equal("Laura Restrepo", resultado.Contactos.First().Gestor!.Nombre);
    }

    [Fact]
    public async Task ObtenerConContactos_ConPacienteInexistente_LanzaPacienteNoEncontradoException()
    {
        using var context = CrearContexto();
        var service = new PacienteService(context);

        await Assert.ThrowsAsync<PacienteNoEncontradoException>(() => service.ObtenerConContactosAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task Listar_DevuelvePacientesOrdenadosPorNombre()
    {
        using var context = CrearContexto();
        context.Pacientes.AddRange(
            new Paciente { Id = Guid.NewGuid(), Nombre = "Zoe Ramirez", DocumentoIdentidad = "1", Telefono = "1", Ciudad = "Lima", FechaInicioTratamiento = new DateOnly(2026, 6, 1), Estado = EstadoPaciente.Activo, FechaCreacion = DateTime.UtcNow },
            new Paciente { Id = Guid.NewGuid(), Nombre = "Ana Torres", DocumentoIdentidad = "2", Telefono = "2", Ciudad = "Quito", FechaInicioTratamiento = new DateOnly(2026, 6, 1), Estado = EstadoPaciente.Activo, FechaCreacion = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();
        var service = new PacienteService(context);

        var resultado = await service.ListarAsync();

        Assert.Equal(2, resultado.Count);
        Assert.Equal("Ana Torres", resultado.First().Nombre);
    }
}
