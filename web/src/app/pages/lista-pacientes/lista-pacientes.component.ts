import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Paciente } from '../../models/paciente.model';
import { PacienteService } from '../../services/paciente.service';

@Component({
  selector: 'app-lista-pacientes',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './lista-pacientes.component.html'
})
export class ListaPacientesComponent implements OnInit {
  pacientes: Paciente[] = [];
  cargando = true;
  error = false;

  constructor(private readonly pacienteService: PacienteService) {}

  ngOnInit(): void {
    this.pacienteService.listar().subscribe({
      next: (pacientes) => {
        this.pacientes = pacientes;
        this.cargando = false;
      },
      error: () => {
        this.error = true;
        this.cargando = false;
      }
    });
  }
}
