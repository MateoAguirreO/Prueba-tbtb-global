using Api.Data;
using Api.Dtos;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public interface IPacienteService
{
    Task<Paciente> CrearAsync(CrearPacienteDto dto);
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
}
