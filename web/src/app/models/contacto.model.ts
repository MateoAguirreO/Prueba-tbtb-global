export type Canal = 'Llamada' | 'WhatsApp' | 'Correo';
export type Resultado = 'Contactado' | 'NoContesta' | 'Rechazada' | 'DatoInvalido';

export interface CrearContactoRequest {
  fecha: string;
  canal: Canal;
  resultado: Resultado;
  gestorId: string;
}

export interface CorregirContactoRequest {
  gestorId: string;
  motivo: string;
  fecha?: string;
  canal?: Canal;
  resultado?: Resultado;
}

export interface Contacto {
  id: string;
  pacienteId: string;
  gestorId: string;
  fecha: string;
  canal: string;
  resultado: string;
}

export interface ContactoHistorial {
  campoModificado: string;
  valorAnterior: string;
  valorNuevo: string;
  motivo: string;
  gestorNombre: string;
  fechaCambio: string;
}
