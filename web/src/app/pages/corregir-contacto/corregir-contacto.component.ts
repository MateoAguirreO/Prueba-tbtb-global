import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Canal, ContactoHistorial, Resultado } from '../../models/contacto.model';
import { Gestor } from '../../models/gestor.model';
import { ContactoService } from '../../services/contacto.service';
import { GestorService } from '../../services/gestor.service';

@Component({
  selector: 'app-corregir-contacto',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './corregir-contacto.component.html'
})
export class CorregirContactoComponent implements OnInit {
  gestores: Gestor[] = [];
  historial: ContactoHistorial[] = [];
  cargandoHistorial = true;

  enviando = false;
  errorMensaje: string | null = null;
  guardadoOk = false;

  canales: Canal[] = ['Llamada', 'WhatsApp', 'Correo'];
  resultados: Resultado[] = ['Contactado', 'NoContesta', 'Rechazada', 'DatoInvalido'];

  form = this.fb.nonNullable.group({
    gestorId: ['', Validators.required],
    motivo: ['', Validators.required],
    fecha: [''],
    canal: [''],
    resultado: ['']
  });

  private contactoId!: string;
  pacienteId: string | null = null;

  constructor(
    private readonly fb: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly contactoService: ContactoService,
    private readonly gestorService: GestorService
  ) {}

  ngOnInit(): void {
    this.contactoId = this.route.snapshot.paramMap.get('id')!;
    // Llega por query param desde el link "corregir" del detalle de paciente,
    // así el link de "volver" funciona desde el principio, no solo tras guardar.
    this.pacienteId = this.route.snapshot.queryParamMap.get('pacienteId');
    this.gestorService.listar().subscribe((gestores) => (this.gestores = gestores));
    this.cargarHistorial();
  }

  private cargarHistorial(): void {
    this.cargandoHistorial = true;
    this.contactoService.obtenerHistorial(this.contactoId).subscribe({
      next: (historial) => {
        this.historial = historial;
        this.cargandoHistorial = false;
      },
      error: () => {
        this.cargandoHistorial = false;
      }
    });
  }

  corregir(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.errorMensaje = 'Completa los campos obligatorios (marcados en rojo): gestor y motivo.';
      return;
    }

    const valores = this.form.getRawValue();
    if (!valores.fecha && !valores.canal && !valores.resultado) {
      this.errorMensaje = 'Indica al menos un campo a corregir (fecha, canal o resultado).';
      return;
    }

    this.errorMensaje = null;
    this.guardadoOk = false;
    this.enviando = true;

    this.contactoService
      .corregir(this.contactoId, {
        gestorId: valores.gestorId,
        motivo: valores.motivo,
        fecha: valores.fecha || undefined,
        canal: (valores.canal as Canal) || undefined,
        resultado: (valores.resultado as Resultado) || undefined
      })
      .subscribe({
        next: (contacto) => {
          this.enviando = false;
          this.guardadoOk = true;
          this.pacienteId = contacto.pacienteId;
          this.form.patchValue({ motivo: '', fecha: '', canal: '', resultado: '' });
          this.cargarHistorial();
        },
        error: (error: HttpErrorResponse) => {
          this.enviando = false;
          this.errorMensaje =
            error.status === 404
              ? 'El contacto no existe.'
              : 'No se pudo corregir el contacto. Revisa los datos.';
        }
      });
  }
}
