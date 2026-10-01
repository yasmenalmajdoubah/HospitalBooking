import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { BookingApiService } from '../../core/booking-api.service';
import { DepartmentDto, DoctorDto } from '../../core/models';

@Component({
  selector: 'app-manage',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './manage.component.html',
  styleUrl: './manage.component.scss'
})
export class ManageComponent implements OnInit {
  private readonly api = inject(BookingApiService);

  departments = signal<DepartmentDto[]>([]);
  doctors = signal<DoctorDto[]>([]);
  message = signal<string | null>(null);
  error = signal<string | null>(null);

  deptForm = { name: '', nameAr: '', description: '' };
  doctorForm = {
    userName: '',
    email: '',
    password: 'Doctor@123',
    fullName: '',
    fullNameAr: '',
    departmentId: 0,
    specialty: '',
    specialtyAr: '',
    licenseNumber: ''
  };

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    this.api.getDepartments().subscribe({
      next: (items) => {
        this.departments.set(items);
        if (!this.doctorForm.departmentId && items.length) {
          this.doctorForm.departmentId = items[0].id;
        }
      },
      error: (err) => this.error.set(err?.error?.message || 'تعذر تحميل الأقسام')
    });

    this.api.getDoctors().subscribe({
      next: (items) => this.doctors.set(items),
      error: (err) => this.error.set(err?.error?.message || 'تعذر تحميل الأطباء')
    });
  }

  addDepartment(): void {
    this.message.set(null);
    this.error.set(null);
    this.api.createDepartment(this.deptForm).subscribe({
      next: () => {
        this.message.set('تمت إضافة القسم.');
        this.deptForm = { name: '', nameAr: '', description: '' };
        this.reload();
      },
      error: (err) => this.error.set(err?.error?.message || 'فشل إضافة القسم')
    });
  }

  addDoctor(): void {
    this.message.set(null);
    this.error.set(null);

    if (!this.doctorForm.departmentId || this.doctorForm.departmentId <= 0) {
      this.error.set('اختر قسماً أولاً.');
      return;
    }

    this.api.createDoctor(this.doctorForm).subscribe({
      next: () => {
        this.message.set('تمت إضافة الطبيب.');
        this.doctorForm = {
          userName: '',
          email: '',
          password: 'Doctor@123',
          fullName: '',
          fullNameAr: '',
          departmentId: this.departments()[0]?.id || 0,
          specialty: '',
          specialtyAr: '',
          licenseNumber: ''
        };
        this.reload();
      },
      error: (err) => this.error.set(err?.error?.message || 'فشل إضافة الطبيب')
    });
  }
}
