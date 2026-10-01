import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { BookingApiService } from '../../core/booking-api.service';
import { DepartmentDto, DoctorDto, PatientListItemDto } from '../../core/models';

@Component({
  selector: 'app-book',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './book.component.html',
  styleUrl: './book.component.scss'
})
export class BookComponent implements OnInit {
  private readonly api = inject(BookingApiService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  departments = signal<DepartmentDto[]>([]);
  doctors = signal<DoctorDto[]>([]);
  patients = signal<PatientListItemDto[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  success = signal<string | null>(null);

  form = {
    departmentId: 0,
    doctorId: 0,
    appointmentDate: '',
    startTime: '09:00',
    notes: '',
    patientId: 0
  };

  readonly role = this.auth.user()?.role;

  ngOnInit(): void {
    this.api.getDepartments().subscribe({
      next: (items) => this.departments.set(items),
      error: (err) => this.error.set(err?.error?.message || 'تعذر تحميل الأقسام')
    });

    this.loadDoctors();

    if (this.role === 'Admin' || this.role === 'Receptionist') {
      this.api.getPatients().subscribe({
        next: (items) => {
          this.patients.set(items);
          if (items.length) {
            this.form.patientId = items[0].id;
          }
        },
        error: (err) => this.error.set(err?.error?.message || 'تعذر تحميل المرضى')
      });
    }
  }

  onDepartmentChange(): void {
    this.form.doctorId = 0;
    this.loadDoctors(this.form.departmentId || undefined);
  }

  loadDoctors(departmentId?: number): void {
    this.api.getDoctors(departmentId).subscribe({
      next: (items) => this.doctors.set(items),
      error: (err) => this.error.set(err?.error?.message || 'تعذر تحميل الأطباء')
    });
  }

  submit(): void {
    if (!this.form.doctorId || this.form.doctorId <= 0) {
      this.error.set('اختر طبيباً.');
      return;
    }

    if (!this.form.appointmentDate || !this.form.startTime) {
      this.error.set('أكمل التاريخ والوقت.');
      return;
    }

    if ((this.role === 'Admin' || this.role === 'Receptionist') && (!this.form.patientId || this.form.patientId <= 0)) {
      this.error.set('اختر مريضاً صالحاً (رقم أكبر من صفر).');
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    this.success.set(null);

    const payload: Record<string, unknown> = {
      doctorId: Number(this.form.doctorId),
      appointmentDate: this.form.appointmentDate,
      startTime: this.normalizeTime(this.form.startTime),
      notes: this.form.notes || undefined
    };

    if (this.role === 'Admin' || this.role === 'Receptionist') {
      payload['patientId'] = Number(this.form.patientId);
    }

    this.api.createAppointment(payload as never).subscribe({
      next: () => {
        this.loading.set(false);
        this.success.set('تم إنشاء الحجز بنجاح.');
        setTimeout(() => void this.router.navigateByUrl('/appointments'), 700);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err?.error?.message || 'فشل إنشاء الحجز');
      }
    });
  }

  private normalizeTime(value: string): string {
    return value.length === 5 ? `${value}:00` : value;
  }
}
