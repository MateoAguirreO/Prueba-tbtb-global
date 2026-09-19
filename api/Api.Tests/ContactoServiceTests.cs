using Api.Data;
using Api.Dtos;
using Api.Models;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Api.Tests;

public class ContactoServiceTests
{
    private static AppDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static async Task<(Paciente paciente, Gestor gestor)> SembrarPacienteYGestor(AppDbContext context)
    {
        var paciente = new Paciente
        {
            Id = Guid.NewGuid(),
            Nombre = "Maria Fernanda Gomez",
            DocumentoIdentidad = "1032456789",
            Telefono = "3001112233",
            Ciudad = "Bogota",
            FechaInicioTratamiento = new DateOnly(2026, 6, 1),
            Estado = EstadoPaciente.Activo,
            FechaCreacion = DateTime.UtcNow
        };
        var gestor = new Gestor { Id = Guid.NewGuid(), Nombre = "Laura Restrepo" };

        context.Pacientes.Add(paciente);
        context.Gestores.Add(gestor);
        await context.SaveChangesAsync();

        return (paciente, gestor);
    }

    [Fact]
    public async Task CA2_RegistrarContacto_ConDatosValidos_QuedaAsociadoAlPaciente()
    {
        using var context = CrearContexto();
        var (paciente, gestor) = await SembrarPacienteYGestor(context);
        var service = new ContactoService(context);

        var dto = new CrearContactoDto
        {
            Fecha = new DateOnly(2026, 7, 1),
            Canal = CanalContacto.Llamada,
            Resultado = ResultadoContacto.Contactado,
            GestorId = gestor.Id
        };

        var contacto = await service.CrearAsync(paciente.Id, dto);

        Assert.Equal(paciente.Id, contacto.PacienteId);
        Assert.Equal(gestor.Id, contacto.GestorId);
        Assert.Single(context.Contactos);
    }

    [Fact]
    public async Task CA2_RegistrarContacto_ConPacienteInexistente_LanzaPacienteNoEncontradoException()
    {
        using var context = CrearContexto();
        var (_, gestor) = await SembrarPacienteYGestor(context);
        var service = new ContactoService(context);

        var dto = new CrearContactoDto
        {
            Fecha = new DateOnly(2026, 7, 1),
            Canal = CanalContacto.Llamada,
            Resultado = ResultadoContacto.Contactado,
            GestorId = gestor.Id
        };

        await Assert.ThrowsAsync<PacienteNoEncontradoException>(() => service.CrearAsync(Guid.NewGuid(), dto));
    }

    [Fact]
    public async Task CA2_RegistrarContacto_ConGestorInexistente_LanzaGestorNoEncontradoException()
    {
        using var context = CrearContexto();
        var (paciente, _) = await SembrarPacienteYGestor(context);
        var service = new ContactoService(context);

        var dto = new CrearContactoDto
        {
            Fecha = new DateOnly(2026, 7, 1),
            Canal = CanalContacto.Llamada,
            Resultado = ResultadoContacto.Contactado,
            GestorId = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<GestorNoEncontradoException>(() => service.CrearAsync(paciente.Id, dto));
    }
}
