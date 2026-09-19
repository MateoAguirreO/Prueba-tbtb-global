namespace Api.Services;

public class DocumentoDuplicadoException : Exception
{
    public DocumentoDuplicadoException(string documentoIdentidad)
        : base($"Ya existe un paciente con el documento {documentoIdentidad}.") { }
}
