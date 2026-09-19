namespace Api.Models;

public enum EstadoPaciente
{
    Incompleto,
    Activo
}

public class Paciente
{
    public Guid Id { get; set; }
    public required string Nombre { get; set; }
    public required string DocumentoIdentidad { get; set; }
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public required string Ciudad { get; set; }
    public DateOnly FechaInicioTratamiento { get; set; }
    public EstadoPaciente Estado { get; set; } = EstadoPaciente.Incompleto;
    public DateTime FechaCreacion { get; set; }

    public ICollection<Contacto> Contactos { get; set; } = new List<Contacto>();
}
