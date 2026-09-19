import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Gestor } from '../models/gestor.model';

@Injectable({ providedIn: 'root' })
export class GestorService {
  constructor(private readonly http: HttpClient) {}

  listar(): Observable<Gestor[]> {
    return this.http.get<Gestor[]>('/api/gestores');
  }
}
