using Api.Data;
using Api.Dtos;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public interface IContactoService
{
    Task<Contacto> CrearAsync(Guid pacienteId, CrearContactoDto dto);
    Task<Contacto> CorregirAsync(Guid contactoId, CorregirContactoDto dto);
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

    public async Task<Contacto> CorregirAsync(Guid contactoId, CorregirContactoDto dto)
    {
        var contacto = await _context.Contactos.FirstOrDefaultAsync(c => c.Id == contactoId);
        if (contacto is null)
        {
            throw new ContactoNoEncontradoException(contactoId);
        }

        var gestorExiste = await _context.Gestores.AnyAsync(g => g.Id == dto.GestorId);
        if (!gestorExiste)
        {
            throw new GestorNoEncontradoException(dto.GestorId);
        }

        var ahora = DateTime.UtcNow;

        // Cada campo que realmente cambia deja su propia fila en ContactoHistorial
        // antes de aplicarse. Contacto nunca se corrige con un UPDATE silencioso.
        if (dto.Fecha.HasValue && dto.Fecha.Value != contacto.Fecha)
        {
            _context.ContactoHistoriales.Add(new ContactoHistorial
            {
                Id = Guid.NewGuid(),
                ContactoId = contacto.Id,
                GestorId = dto.GestorId,
                CampoModificado = nameof(Contacto.Fecha),
                ValorAnterior = contacto.Fecha.ToString("O"),
                ValorNuevo = dto.Fecha.Value.ToString("O"),
                Motivo = dto.Motivo,
                FechaCambio = ahora
            });
            contacto.Fecha = dto.Fecha.Value;
        }

        if (dto.Canal.HasValue && dto.Canal.Value != contacto.Canal)
        {
            _context.ContactoHistoriales.Add(new ContactoHistorial
            {
                Id = Guid.NewGuid(),
                ContactoId = contacto.Id,
                GestorId = dto.GestorId,
                CampoModificado = nameof(Contacto.Canal),
                ValorAnterior = contacto.Canal.ToString(),
                ValorNuevo = dto.Canal.Value.ToString(),
                Motivo = dto.Motivo,
                FechaCambio = ahora
            });
            contacto.Canal = dto.Canal.Value;
        }

        if (dto.Resultado.HasValue && dto.Resultado.Value != contacto.Resultado)
        {
            _context.ContactoHistoriales.Add(new ContactoHistorial
            {
                Id = Guid.NewGuid(),
                ContactoId = contacto.Id,
                GestorId = dto.GestorId,
                CampoModificado = nameof(Contacto.Resultado),
                ValorAnterior = contacto.Resultado.ToString(),
                ValorNuevo = dto.Resultado.Value.ToString(),
                Motivo = dto.Motivo,
                FechaCambio = ahora
            });
            contacto.Resultado = dto.Resultado.Value;
        }

        await _context.SaveChangesAsync();

        return contacto;
    }
}
