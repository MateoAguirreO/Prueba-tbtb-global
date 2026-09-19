using Api.Data;
using Api.Dtos;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public interface IPacienteService
{
    Task<Paciente> CrearAsync(CrearPacienteDto dto);
    Task<Paciente> ObtenerConContactosAsync(Guid pacienteId);
}

public class PacienteService : IPacienteService
{
    private readonly AppDbContext _context;

    public PacienteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Paciente> CrearAsync(CrearPacienteDto dto)
    {
        var existe = await _context.Pacientes.AnyAsync(p => p.DocumentoIdentidad == dto.DocumentoIdentidad);
        if (existe)
        {
            throw new DocumentoDuplicadoException(dto.DocumentoIdentidad);
        }

        // Telefono es obligatorio en este DTO (01-hallazgos.md, hallazgo 1), por
        // eso un paciente creado por este flujo siempre nace Activo.
        var paciente = new Paciente
        {
            Id = Guid.NewGuid(),
            Nombre = dto.Nombre,
            DocumentoIdentidad = dto.DocumentoIdentidad,
            Telefono = dto.Telefono,
            Correo = dto.Correo,
            Ciudad = dto.Ciudad,
            FechaInicioTratamiento = dto.FechaInicioTratamiento,
            Estado = EstadoPaciente.Activo,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        return paciente;
    }

    // Consulta con criterio (02-plan.md § 4): junta Paciente, Contacto y Gestor
    // para la pantalla de detalle. El orden por Fecha usa IX_Contacto_PacienteId_Fecha.
    public async Task<Paciente> ObtenerConContactosAsync(Guid pacienteId)
    {
        var paciente = await _context.Pacientes
            // Solo lectura: si el paciente ya estuviera trackeado en este contexto por
            // otra operación previa en el mismo request, la resolución de identidad de
            // EF Core podría devolver la colección sin respetar el ORDER BY del SQL.
            .AsNoTracking()
            .Include(p => p.Contactos.OrderByDescending(c => c.Fecha))
                .ThenInclude(c => c.Gestor)
            .FirstOrDefaultAsync(p => p.Id == pacienteId);

        if (paciente is null)
        {
            throw new PacienteNoEncontradoException(pacienteId);
        }

        return paciente;
    }
}
