export interface CrearPacienteRequest {
  nombre: string;
  documentoIdentidad: string;
  telefono: string;
  correo?: string;
  ciudad: string;
  fechaInicioTratamiento: string;
}

export interface Paciente {
  id: string;
  nombre: string;
  documentoIdentidad: string;
  telefono: string | null;
  correo: string | null;
  ciudad: string;
  fechaInicioTratamiento: string;
  estado: string;
}

export interface ContactoConGestor {
  id: string;
  fecha: string;
  canal: string;
  resultado: string;
  gestorNombre: string;
}

export interface PacienteConContactos {
  id: string;
  nombre: string;
  documentoIdentidad: string;
  telefono: string | null;
  ciudad: string;
  estado: string;
  contactos: ContactoConGestor[];
}
