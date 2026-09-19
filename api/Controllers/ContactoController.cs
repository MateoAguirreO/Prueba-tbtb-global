using Api.Dtos;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/contactos")]
public class ContactoController : ControllerBase
{
    private readonly IContactoService _service;

    public ContactoController(IContactoService service)
    {
        _service = service;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Corregir(Guid id, CorregirContactoDto dto)
    {
        try
        {
            var contacto = await _service.CorregirAsync(id, dto);
            var response = new ContactoDto
            {
                Id = contacto.Id,
                PacienteId = contacto.PacienteId,
                GestorId = contacto.GestorId,
                Fecha = contacto.Fecha,
                Canal = contacto.Canal.ToString(),
                Resultado = contacto.Resultado.ToString()
            };
            return Ok(response);
        }
        catch (ContactoNoEncontradoException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (GestorNoEncontradoException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
