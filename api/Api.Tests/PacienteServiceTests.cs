using Api.Data;
using Api.Dtos;
using Api.Models;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Api.Tests;

public class PacienteServiceTests
{
    private static AppDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static CrearPacienteDto DtoValido() => new()
    {
        Nombre = "Maria Fernanda Gomez",
        DocumentoIdentidad = "1032456789",
        Telefono = "3001112233",
        Ciudad = "Bogota",
        FechaInicioTratamiento = new DateOnly(2026, 6, 1)
    };

    [Fact]
    public async Task CA1_CrearPaciente_ConDatosValidos_QuedaActivoYDisponibleParaAgendarContactos()
    {
        using var context = CrearContexto();
        var service = new PacienteService(context);

        var paciente = await service.CrearAsync(DtoValido());

        Assert.NotEqual(Guid.Empty, paciente.Id);
        Assert.Equal(EstadoPaciente.Activo, paciente.Estado);
        Assert.Single(context.Pacientes);
    }

    [Fact]
    public async Task CA1_CrearPaciente_ConDocumentoDuplicado_LanzaDocumentoDuplicadoException()
    {
        using var context = CrearContexto();
        var service = new PacienteService(context);
        await service.CrearAsync(DtoValido());

        await Assert.ThrowsAsync<DocumentoDuplicadoException>(() => service.CrearAsync(DtoValido()));
    }
}
