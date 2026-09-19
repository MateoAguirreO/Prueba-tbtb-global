import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Contacto, ContactoHistorial, CorregirContactoRequest, CrearContactoRequest } from '../models/contacto.model';

@Injectable({ providedIn: 'root' })
export class ContactoService {
  constructor(private readonly http: HttpClient) {}

  crear(pacienteId: string, request: CrearContactoRequest): Observable<Contacto> {
    return this.http.post<Contacto>(`/api/pacientes/${pacienteId}/contactos`, request);
  }

  corregir(contactoId: string, request: CorregirContactoRequest): Observable<Contacto> {
    return this.http.put<Contacto>(`/api/contactos/${contactoId}`, request);
  }

  obtenerHistorial(contactoId: string): Observable<ContactoHistorial[]> {
    return this.http.get<ContactoHistorial[]>(`/api/contactos/${contactoId}/historial`);
  }
}
