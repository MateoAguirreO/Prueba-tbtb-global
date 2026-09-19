using Api.Dtos;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/pacientes")]
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _service;

    public PacientesController(IPacienteService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearPacienteDto dto)
    {
        try
        {
            var paciente = await _service.CrearAsync(dto);
            var response = new PacienteDto
            {
                Id = paciente.Id,
                Nombre = paciente.Nombre,
                DocumentoIdentidad = paciente.DocumentoIdentidad,
                Telefono = paciente.Telefono,
                Correo = paciente.Correo,
                Ciudad = paciente.Ciudad,
                FechaInicioTratamiento = paciente.FechaInicioTratamiento,
                Estado = paciente.Estado.ToString()
            };
            return Created($"/api/pacientes/{paciente.Id}", response);
        }
        catch (DocumentoDuplicadoException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var pacientes = await _service.ListarAsync();
        var response = pacientes.Select(p => new PacienteDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            DocumentoIdentidad = p.DocumentoIdentidad,
            Telefono = p.Telefono,
            Correo = p.Correo,
            Ciudad = p.Ciudad,
            FechaInicioTratamiento = p.FechaInicioTratamiento,
            Estado = p.Estado.ToString()
        }).ToList();
        return Ok(response);
    }

    [HttpGet("{id}/contactos")]
    public async Task<IActionResult> ObtenerConContactos(Guid id)
    {
        try
        {
            var paciente = await _service.ObtenerConContactosAsync(id);
            var response = new PacienteConContactosDto
            {
                Id = paciente.Id,
                Nombre = paciente.Nombre,
                DocumentoIdentidad = paciente.DocumentoIdentidad,
                Telefono = paciente.Telefono,
                Ciudad = paciente.Ciudad,
                Estado = paciente.Estado.ToString(),
                Contactos = paciente.Contactos.Select(c => new ContactoConGestorDto
                {
                    Id = c.Id,
                    Fecha = c.Fecha,
                    Canal = c.Canal.ToString(),
                    Resultado = c.Resultado.ToString(),
                    GestorNombre = c.Gestor!.Nombre
                }).ToList()
            };
            return Ok(response);
        }
        catch (PacienteNoEncontradoException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
