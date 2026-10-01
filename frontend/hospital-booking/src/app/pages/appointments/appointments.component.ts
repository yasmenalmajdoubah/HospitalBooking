import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { BookingApiService } from '../../core/booking-api.service';
import { AppointmentDto } from '../../core/models';

@Component({
  selector: 'app-appointments',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './appointments.component.html',
  styleUrl: './appointments.component.scss'
})
export class AppointmentsComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly api = inject(BookingApiService);

  readonly user = this.auth.user;
  readonly appointments = signal<AppointmentDto[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly actionMsg = signal<string | null>(null);

  readonly canBook = computed(() => {
    const role = this.user()?.role;
    return role === 'Admin' || role === 'Receptionist' || role === 'Patient';
  });

  readonly isAdmin = computed(() => this.user()?.role === 'Admin');

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    this.loading.set(true);
    this.error.set(null);
    this.api.getAppointments().subscribe({
      next: (items) => {
        this.appointments.set(items);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err?.error?.message || 'تعذر تحميل الحجوزات. تأكد أن الـ API والداتا بيس شغالين.');
      }
    });
  }

  cancel(item: AppointmentDto): void {
    this.api.cancelAppointment(item.id).subscribe({
      next: () => {
        this.actionMsg.set('تم إلغاء الحجز.');
        this.reload();
      },
      error: (err) => this.actionMsg.set(err?.error?.message || 'فشل الإلغاء')
    });
  }

  confirm(item: AppointmentDto): void {
    this.api.updateStatus(item.id, 2).subscribe({
      next: () => {
        this.actionMsg.set('تم تأكيد الحجز.');
        this.reload();
      },
      error: (err) => this.actionMsg.set(err?.error?.message || 'فشل التأكيد')
    });
  }

  complete(item: AppointmentDto): void {
    this.api.updateStatus(item.id, 4).subscribe({
      next: () => {
        this.actionMsg.set('تم إكمال الحجز.');
        this.reload();
      },
      error: (err) => this.actionMsg.set(err?.error?.message || 'فشل الإكمال')
    });
  }

  statusClass(status: string): string {
    return status.toLowerCase();
  }

  logout(): void {
    this.auth.logout();
  }
}
