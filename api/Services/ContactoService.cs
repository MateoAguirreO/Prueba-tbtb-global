using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Dtos;
using Api.Models;

namespace Api.Services;

public interface IContactoService
{
    Task<Contacto> CrearAsync(Guid pacienteId, CrearContactoDto dto);
}

public class ContactoService : IContactoService
{
    private readonly AppDbContext _context;

    public ContactoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Contacto> CrearAsync(Guid pacienteId, CrearContactoDto dto)
    {
        var pacienteExiste = await _context.Pacientes.AnyAsync(p => p.Id == pacienteId);
        if (!pacienteExiste)
        {
            throw new PacienteNoEncontradoException(pacienteId);
        }

        var gestorExiste = await _context.Gestores.AnyAsync(g => g.Id == dto.GestorId);
        if (!gestorExiste)
        {
            throw new GestorNoEncontradoException(dto.GestorId);
        }

        var contacto = new Contacto
        {
            Id = Guid.NewGuid(),
            PacienteId = pacienteId,
            GestorId = dto.GestorId,
            Fecha = dto.Fecha,
            Canal = dto.Canal,
            Resultado = dto.Resultado,
            FechaRegistro = DateTime.UtcNow
        };

        _context.Contactos.Add(contacto);
        await _context.SaveChangesAsync();

        return contacto;
    }
}
