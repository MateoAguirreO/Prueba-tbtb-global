namespace Api.Models;

public enum CanalContacto
{
    Llamada,
    WhatsApp,
    Correo
}

public enum ResultadoContacto
{
    Contactado,
    NoContesta,
    Rechazada,
    DatoInvalido
}

public class Contacto
{
    public Guid Id { get; set; }
    public Guid PacienteId { get; set; }
    public Paciente? Paciente { get; set; }
    public Guid GestorId { get; set; }
    public Gestor? Gestor { get; set; }
    public DateOnly Fecha { get; set; }
    public CanalContacto Canal { get; set; }
    public ResultadoContacto Resultado { get; set; }
    public DateTime FechaRegistro { get; set; }
}
