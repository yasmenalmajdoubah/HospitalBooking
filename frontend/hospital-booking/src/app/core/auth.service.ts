import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { AuthResponse, LoginRequest, RegisterPatientRequest, UserDto } from './models';

const TOKEN_KEY = 'hb_token';
const USER_KEY = 'hb_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly userSignal = signal<UserDto | null>(this.readUser());
  readonly user = this.userSignal.asReadonly();
  readonly isLoggedIn = computed(() => !!this.userSignal() && !!this.getToken());

  constructor(
    private readonly http: HttpClient,
    private readonly router: Router
  ) {}

  login(payload: LoginRequest) {
    return this.http.post<AuthResponse>('/api/auth/login', payload).pipe(
      tap((res) => this.persist(res))
    );
  }

  registerPatient(payload: RegisterPatientRequest) {
    return this.http.post<AuthResponse>('/api/auth/register-patient', payload).pipe(
      tap((res) => this.persist(res))
    );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.userSignal.set(null);
    void this.router.navigateByUrl('/login');
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  private persist(res: AuthResponse): void {
    localStorage.setItem(TOKEN_KEY, res.accessToken);
    localStorage.setItem(USER_KEY, JSON.stringify(res.user));
    this.userSignal.set(res.user);
  }

  private readUser(): UserDto | null {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) {
      return null;
    }

    try {
      return JSON.parse(raw) as UserDto;
    } catch {
      return null;
    }
  }
}
