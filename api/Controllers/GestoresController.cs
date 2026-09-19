using Api.Data;
using Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

// Lista simple de referencia para poblar el selector de gestor al registrar o
// corregir un contacto. No hay login: cualquier gestor sembrado es válido.
[ApiController]
[Route("api/gestores")]
public class GestoresController : ControllerBase
{
    private readonly AppDbContext _context;

    public GestoresController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var gestores = await _context.Gestores
            .Select(g => new GestorDto { Id = g.Id, Nombre = g.Nombre })
            .OrderBy(g => g.Nombre)
            .ToListAsync();

        return Ok(gestores);
    }
}
