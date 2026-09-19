using System.ComponentModel.DataAnnotations;
using Api.Models;

namespace Api.Dtos;

public class CrearContactoDto
{
    [Required]
    public DateOnly Fecha { get; set; }

    [Required]
    public CanalContacto Canal { get; set; }

    [Required]
    public ResultadoContacto Resultado { get; set; }

    [Required]
    public Guid GestorId { get; set; }
}

public class ContactoDto
{
    public Guid Id { get; set; }
    public Guid PacienteId { get; set; }
    public Guid GestorId { get; set; }
    public DateOnly Fecha { get; set; }
    public string Canal { get; set; } = string.Empty;
    public string Resultado { get; set; } = string.Empty;
}
