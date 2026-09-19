export interface PaisTelefono {
  codigo: string;
  bandera: string;
  nombre: string;
}

// El programa opera solo en estos 3 países (ver anexo del PRD).
export const PAISES_TELEFONO: PaisTelefono[] = [
  { codigo: '+57', bandera: '🇨🇴', nombre: 'Colombia' },
  { codigo: '+51', bandera: '🇵🇪', nombre: 'Perú' },
  { codigo: '+593', bandera: '🇪🇨', nombre: 'Ecuador' }
];
