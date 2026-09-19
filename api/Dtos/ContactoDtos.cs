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

public class CorregirContactoDto : IValidatableObject
{
    [Required]
    public Guid GestorId { get; set; }

    [Required, MaxLength(300)]
    public string Motivo { get; set; } = string.Empty;

    public DateOnly? Fecha { get; set; }
    public CanalContacto? Canal { get; set; }
    public ResultadoContacto? Resultado { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Fecha is null && Canal is null && Resultado is null)
        {
            yield return new ValidationResult(
                "Debe indicar al menos un campo a corregir (fecha, canal o resultado).",
                new[] { nameof(Fecha), nameof(Canal), nameof(Resultado) });
        }
    }
}
