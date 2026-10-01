import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  AppointmentDto,
  AppointmentStatusDto,
  CreateAppointmentRequest,
  CreateDepartmentRequest,
  CreateDoctorRequest,
  DepartmentDto,
  DoctorDto,
  PatientListItemDto
} from './models';

@Injectable({ providedIn: 'root' })
export class BookingApiService {
  constructor(private readonly http: HttpClient) {}

  getAppointments(filters?: Record<string, string | number | undefined>) {
    let params = new HttpParams();
    if (filters) {
      Object.entries(filters).forEach(([key, value]) => {
        if (value !== undefined && value !== null && `${value}` !== '') {
          params = params.set(key, String(value));
        }
      });
    }
    return this.http.get<AppointmentDto[]>('/api/appointments', { params });
  }

  createAppointment(payload: CreateAppointmentRequest) {
    return this.http.post<AppointmentDto>('/api/appointments', payload);
  }

  cancelAppointment(id: number) {
    return this.http.post<AppointmentDto>(`/api/appointments/${id}/cancel`, {});
  }

  updateStatus(id: number, statusId: number) {
    return this.http.patch<AppointmentDto>(`/api/appointments/${id}/status`, { statusId });
  }

  getDepartments() {
    return this.http.get<DepartmentDto[]>('/api/catalog/departments');
  }

  createDepartment(payload: CreateDepartmentRequest) {
    return this.http.post<DepartmentDto>('/api/catalog/departments', payload);
  }

  getDoctors(departmentId?: number) {
    let params = new HttpParams();
    if (departmentId) {
      params = params.set('departmentId', departmentId);
    }
    return this.http.get<DoctorDto[]>('/api/catalog/doctors', { params });
  }

  createDoctor(payload: CreateDoctorRequest) {
    return this.http.post<DoctorDto>('/api/catalog/doctors', payload);
  }

  getPatients() {
    return this.http.get<PatientListItemDto[]>('/api/catalog/patients');
  }

  getStatuses() {
    return this.http.get<AppointmentStatusDto[]>('/api/catalog/appointment-statuses');
  }
}
