import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Canal, Resultado } from '../../models/contacto.model';
import { Gestor } from '../../models/gestor.model';
import { PacienteConContactos } from '../../models/paciente.model';
import { ContactoService } from '../../services/contacto.service';
import { GestorService } from '../../services/gestor.service';
import { PacienteService } from '../../services/paciente.service';

@Component({
  selector: 'app-detalle-paciente',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './detalle-paciente.component.html'
})
export class DetallePacienteComponent implements OnInit {
  paciente: PacienteConContactos | null = null;
  gestores: Gestor[] = [];
  cargando = true;
  errorCarga: string | null = null;

  enviandoContacto = false;
  errorContacto: string | null = null;

  canales: Canal[] = ['Llamada', 'WhatsApp', 'Correo'];
  resultados: Resultado[] = ['Contactado', 'NoContesta', 'Rechazada', 'DatoInvalido'];

  form = this.fb.nonNullable.group({
    fecha: ['', Validators.required],
    canal: ['Llamada' as Canal, Validators.required],
    resultado: ['Contactado' as Resultado, Validators.required],
    gestorId: ['', Validators.required]
  });

  private pacienteId!: string;

  constructor(
    private readonly fb: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly pacienteService: PacienteService,
    private readonly contactoService: ContactoService,
    private readonly gestorService: GestorService
  ) {}

  ngOnInit(): void {
    this.pacienteId = this.route.snapshot.paramMap.get('id')!;
    this.cargarPaciente();
    this.gestorService.listar().subscribe((gestores) => (this.gestores = gestores));
  }

  private cargarPaciente(): void {
    this.cargando = true;
    this.pacienteService.obtenerConContactos(this.pacienteId).subscribe({
      next: (paciente) => {
        this.paciente = paciente;
        this.cargando = false;
      },
      error: () => {
        this.errorCarga = 'No se pudo cargar el paciente.';
        this.cargando = false;
      }
    });
  }

  registrarContacto(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.errorContacto = null;
    this.enviandoContacto = true;

    const valores = this.form.getRawValue();
    this.contactoService.crear(this.pacienteId, valores).subscribe({
      next: () => {
        this.enviandoContacto = false;
        this.form.reset({ canal: 'Llamada', resultado: 'Contactado' });
        this.cargarPaciente();
      },
      error: (error: HttpErrorResponse) => {
        this.enviandoContacto = false;
        this.errorContacto =
          error.status === 404
            ? 'El paciente no existe.'
            : 'No se pudo registrar el contacto. Revisa los datos.';
      }
    });
  }
}
