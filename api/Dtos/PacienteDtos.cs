using System.ComponentModel.DataAnnotations;

namespace Api.Dtos;

public class CrearPacienteDto
{
    [Required, MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string DocumentoIdentidad { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Telefono { get; set; } = string.Empty;

    [MaxLength(200), EmailAddress]
    public string? Correo { get; set; }

    [Required, MaxLength(100)]
    public string Ciudad { get; set; } = string.Empty;

    [Required]
    public DateOnly FechaInicioTratamiento { get; set; }
}

public class PacienteDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string DocumentoIdentidad { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public string Ciudad { get; set; } = string.Empty;
    public DateOnly FechaInicioTratamiento { get; set; }
    public string Estado { get; set; } = string.Empty;
}
