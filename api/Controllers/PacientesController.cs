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
}
