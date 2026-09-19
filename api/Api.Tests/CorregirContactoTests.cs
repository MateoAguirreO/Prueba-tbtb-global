using System.ComponentModel.DataAnnotations;
using Api.Data;
using Api.Dtos;
using Api.Models;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Api.Tests;

public class CorregirContactoTests
{
    private static AppDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static async Task<(Paciente paciente, Gestor gestor, Contacto contacto)> SembrarEscenario(AppDbContext context)
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
        var contacto = new Contacto
        {
            Id = Guid.NewGuid(),
            PacienteId = paciente.Id,
            GestorId = gestor.Id,
            Fecha = new DateOnly(2026, 7, 1),
            Canal = CanalContacto.Llamada,
            Resultado = ResultadoContacto.NoContesta,
            FechaRegistro = DateTime.UtcNow
        };

        context.Pacientes.Add(paciente);
        context.Gestores.Add(gestor);
        context.Contactos.Add(contacto);
        await context.SaveChangesAsync();

        return (paciente, gestor, contacto);
    }

    [Fact]
    public async Task CA3_CorregirContacto_ConResultadoDistinto_RegistraHistorialYActualizaContacto()
    {
        using var context = CrearContexto();
        var (_, gestor, contacto) = await SembrarEscenario(context);
        var service = new ContactoService(context);

        var dto = new CorregirContactoDto
        {
            GestorId = gestor.Id,
            Motivo = "El gestor anoto mal el resultado, el paciente si contesto",
            Resultado = ResultadoContacto.Contactado
        };

        var actualizado = await service.CorregirAsync(contacto.Id, dto);

        Assert.Equal(ResultadoContacto.Contactado, actualizado.Resultado);

        var historial = Assert.Single(context.ContactoHistoriales);
        Assert.Equal(nameof(Contacto.Resultado), historial.CampoModificado);
        Assert.Equal("NoContesta", historial.ValorAnterior);
        Assert.Equal("Contactado", historial.ValorNuevo);
        Assert.Equal(gestor.Id, historial.GestorId);
    }

    [Fact]
    public async Task CA3_CorregirContacto_ConContactoInexistente_LanzaContactoNoEncontradoException()
    {
        using var context = CrearContexto();
        var (_, gestor, _) = await SembrarEscenario(context);
        var service = new ContactoService(context);

        var dto = new CorregirContactoDto
        {
            GestorId = gestor.Id,
            Motivo = "Correccion",
            Resultado = ResultadoContacto.Contactado
        };

        await Assert.ThrowsAsync<ContactoNoEncontradoException>(() => service.CorregirAsync(Guid.NewGuid(), dto));
    }

    [Fact]
    public async Task CA3_CorregirContacto_ConGestorInexistente_LanzaGestorNoEncontradoException()
    {
        using var context = CrearContexto();
        var (_, _, contacto) = await SembrarEscenario(context);
        var service = new ContactoService(context);

        var dto = new CorregirContactoDto
        {
            GestorId = Guid.NewGuid(),
            Motivo = "Correccion",
            Resultado = ResultadoContacto.Contactado
        };

        await Assert.ThrowsAsync<GestorNoEncontradoException>(() => service.CorregirAsync(contacto.Id, dto));
    }

    [Fact]
    public async Task ObtenerHistorial_DespuesDeCorregir_DevuelveLaFilaDeAuditoria()
    {
        using var context = CrearContexto();
        var (_, gestor, contacto) = await SembrarEscenario(context);
        var service = new ContactoService(context);

        await service.CorregirAsync(contacto.Id, new CorregirContactoDto
        {
            GestorId = gestor.Id,
            Motivo = "El paciente si contesto",
            Resultado = ResultadoContacto.Contactado
        });

        var historial = await service.ObtenerHistorialAsync(contacto.Id);

        var fila = Assert.Single(historial);
        Assert.Equal("NoContesta", fila.ValorAnterior);
        Assert.Equal("Contactado", fila.ValorNuevo);
        Assert.Equal(gestor.Nombre, fila.Gestor!.Nombre);
    }

    [Fact]
    public async Task ObtenerHistorial_ConContactoInexistente_LanzaContactoNoEncontradoException()
    {
        using var context = CrearContexto();
        var service = new ContactoService(context);

        await Assert.ThrowsAsync<ContactoNoEncontradoException>(() => service.ObtenerHistorialAsync(Guid.NewGuid()));
    }

    [Fact]
    public void CA3_CorregirContactoDto_SinCamposACorregir_FallaValidacion()
    {
        var dto = new CorregirContactoDto
        {
            GestorId = Guid.NewGuid(),
            Motivo = "Sin cambios reales"
        };

        var resultados = dto.Validate(new ValidationContext(dto)).ToList();

        Assert.Single(resultados);
    }
}
