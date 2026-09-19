using Microsoft.AspNetCore.Mvc;
using Api.Dtos;
using Api.Services;

namespace Api.Controllers;

[ApiController]
[Route("api/pacientes/{pacienteId}/contactos")]
public class ContactosController : ControllerBase
{
    private readonly IContactoService _service;

    public ContactosController(IContactoService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Crear(Guid pacienteId, CrearContactoDto dto)
    {
        try
        {
            var contacto = await _service.CrearAsync(pacienteId, dto);
            var response = new ContactoDto
            {
                Id = contacto.Id,
                PacienteId = contacto.PacienteId,
                GestorId = contacto.GestorId,
                Fecha = contacto.Fecha,
                Canal = contacto.Canal.ToString(),
                Resultado = contacto.Resultado.ToString()
            };
            return Created($"/api/pacientes/{pacienteId}/contactos/{contacto.Id}", response);
        }
        catch (PacienteNoEncontradoException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (GestorNoEncontradoException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
