import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { signal } from '@angular/core';
import { AuthService } from '../../core/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  mode: 'login' | 'register' = 'login';
  loading = signal(false);
  error = signal<string | null>(null);

  loginForm = {
    userNameOrEmail: 'admin',
    password: 'Admin@123'
  };

  registerForm = {
    userName: '',
    email: '',
    password: '',
    fullName: '',
    fullNameAr: '',
    phone: ''
  };

  constructor() {
    if (this.auth.isLoggedIn()) {
      void this.router.navigateByUrl('/appointments');
    }
  }

  submitLogin(): void {
    this.loading.set(true);
    this.error.set(null);
    this.auth.login(this.loginForm).subscribe({
      next: () => {
        this.loading.set(false);
        void this.router.navigateByUrl('/appointments');
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err?.error?.message || 'فشل تسجيل الدخول. تأكد أن الـ API والداتا بيس شغالين.');
      }
    });
  }

  submitRegister(): void {
    this.loading.set(true);
    this.error.set(null);
    this.auth.registerPatient(this.registerForm).subscribe({
      next: () => {
        this.loading.set(false);
        void this.router.navigateByUrl('/appointments');
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err?.error?.message || 'فشل التسجيل.');
      }
    });
  }
}
