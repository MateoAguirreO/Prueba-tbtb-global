namespace Api.Dtos;

public class ContactoConGestorDto
{
    public Guid Id { get; set; }
    public DateOnly Fecha { get; set; }
    public string Canal { get; set; } = string.Empty;
    public string Resultado { get; set; } = string.Empty;
    public string GestorNombre { get; set; } = string.Empty;
}

public class PacienteConContactosDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string DocumentoIdentidad { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string Ciudad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public List<ContactoConGestorDto> Contactos { get; set; } = new();
}
