namespace Api.Models;

public class ContactoHistorial
{
    public Guid Id { get; set; }
    public Guid ContactoId { get; set; }
    public Contacto? Contacto { get; set; }
    public Guid GestorId { get; set; }
    public Gestor? Gestor { get; set; }
    public required string CampoModificado { get; set; }
    public required string ValorAnterior { get; set; }
    public required string ValorNuevo { get; set; }
    public required string Motivo { get; set; }
    public DateTime FechaCambio { get; set; }
}
