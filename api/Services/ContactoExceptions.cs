namespace Api.Services;

public class PacienteNoEncontradoException : Exception
{
    public PacienteNoEncontradoException(Guid pacienteId)
        : base($"No existe un paciente con id {pacienteId}.") { }
}

public class GestorNoEncontradoException : Exception
{
    public GestorNoEncontradoException(Guid gestorId)
        : base($"No existe un gestor con id {gestorId}.") { }
}

public class ContactoNoEncontradoException : Exception
{
    public ContactoNoEncontradoException(Guid contactoId)
        : base($"No existe un contacto con id {contactoId}.") { }
}
