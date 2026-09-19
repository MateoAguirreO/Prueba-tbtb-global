import { Routes } from '@angular/router';
import { RegistroPacienteComponent } from './pages/registro-paciente/registro-paciente.component';
import { DetallePacienteComponent } from './pages/detalle-paciente/detalle-paciente.component';
import { CorregirContactoComponent } from './pages/corregir-contacto/corregir-contacto.component';

export const routes: Routes = [
  { path: '', redirectTo: 'pacientes/nuevo', pathMatch: 'full' },
  { path: 'pacientes/nuevo', component: RegistroPacienteComponent },
  { path: 'pacientes/:id', component: DetallePacienteComponent },
  { path: 'contactos/:id/editar', component: CorregirContactoComponent }
];
