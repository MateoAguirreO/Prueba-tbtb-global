import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CIUDADES } from '../../shared/ciudades';
import { PaisTelefono, PAISES_TELEFONO } from '../../shared/paises';
import { PacienteService } from '../../services/paciente.service';

@Component({
  selector: 'app-registro-paciente',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './registro-paciente.component.html'
})
export class RegistroPacienteComponent {
  enviando = false;
  errorMensaje: string | null = null;

  paises = PAISES_TELEFONO;
  ciudades = CIUDADES;

  // <select> nativo no renderiza emoji a color en Windows (cae a texto
  // "co"), así que el selector de país es un dropdown propio en HTML/CSS.
  paisSeleccionado: PaisTelefono = this.paises[0];
  menuPaisesAbierto = false;

  elegirPais(pais: PaisTelefono): void {
    this.paisSeleccionado = pais;
    this.form.controls.codigoPais.setValue(pais.codigo);
    this.menuPaisesAbierto = false;
  }

  form = this.fb.nonNullable.group({
    nombre: ['', Validators.required],
    documentoIdentidad: ['', Validators.required],
    codigoPais: [this.paises[0].codigo, Validators.required],
    telefonoLocal: ['', Validators.required],
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
      this.errorMensaje = 'Completa los campos obligatorios (marcados en rojo).';
      return;
    }

    this.errorMensaje = null;
    this.enviando = true;

    const valores = this.form.getRawValue();
    this.pacienteService
      .crear({
        nombre: valores.nombre,
        documentoIdentidad: valores.documentoIdentidad,
        telefono: `${valores.codigoPais} ${valores.telefonoLocal}`.trim(),
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
