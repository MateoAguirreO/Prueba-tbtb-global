import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { PacienteService } from '../../services/paciente.service';

@Component({
  selector: 'app-registro-paciente',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './registro-paciente.component.html'
})
export class RegistroPacienteComponent {
  enviando = false;
  errorMensaje: string | null = null;

  form = this.fb.nonNullable.group({
    nombre: ['', Validators.required],
    documentoIdentidad: ['', Validators.required],
    telefono: ['', Validators.required],
    correo: [''],
    ciudad: ['', Validators.required],
    fechaInicioTratamiento: ['', Validators.required]
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly pacienteService: PacienteService,
    private readonly router: Router
  ) {}

  guardar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.errorMensaje = null;
    this.enviando = true;

    const valores = this.form.getRawValue();
    this.pacienteService
      .crear({
        nombre: valores.nombre,
        documentoIdentidad: valores.documentoIdentidad,
        telefono: valores.telefono,
        correo: valores.correo || undefined,
        ciudad: valores.ciudad,
        fechaInicioTratamiento: valores.fechaInicioTratamiento
      })
      .subscribe({
        next: (paciente) => {
          this.router.navigate(['/pacientes', paciente.id]);
        },
        error: (error: HttpErrorResponse) => {
          this.enviando = false;
          if (error.status === 409) {
            this.errorMensaje = 'Ya existe un paciente con ese documento de identidad.';
          } else if (error.status === 400) {
            this.errorMensaje = 'Revisa los datos: hay campos inválidos o incompletos.';
          } else {
            this.errorMensaje = 'No se pudo registrar el paciente. Intenta de nuevo.';
          }
        }
      });
  }
}
