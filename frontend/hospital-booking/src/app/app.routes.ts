import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'appointments' },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'appointments',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/appointments/appointments.component').then((m) => m.AppointmentsComponent)
  },
  {
    path: 'book',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/book/book.component').then((m) => m.BookComponent)
  },
  {
    path: 'manage',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/manage/manage.component').then((m) => m.ManageComponent)
  },
  { path: '**', redirectTo: 'appointments' }
];
