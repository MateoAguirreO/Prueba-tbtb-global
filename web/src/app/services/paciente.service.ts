import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CrearPacienteRequest, Paciente, PacienteConContactos } from '../models/paciente.model';

@Injectable({ providedIn: 'root' })
export class PacienteService {
  private readonly baseUrl = '/api/pacientes';

  constructor(private readonly http: HttpClient) {}

  crear(request: CrearPacienteRequest): Observable<Paciente> {
    return this.http.post<Paciente>(this.baseUrl, request);
  }

  obtenerConContactos(id: string): Observable<PacienteConContactos> {
    return this.http.get<PacienteConContactos>(`${this.baseUrl}/${id}/contactos`);
  }
}
