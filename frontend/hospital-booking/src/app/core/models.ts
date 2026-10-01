export interface UserDto {
  id: number;
  userName: string;
  email: string;
  fullName: string;
  fullNameAr?: string | null;
  role: string;
  roleAr: string;
}

export interface AuthResponse {
  accessToken: string;
  expiresAtUtc: string;
  user: UserDto;
}

export interface LoginRequest {
  userNameOrEmail: string;
  password: string;
}

export interface RegisterPatientRequest {
  userName: string;
  email: string;
  password: string;
  fullName: string;
  fullNameAr?: string;
  phone?: string;
  nationalId?: string;
}

export interface AppointmentDto {
  id: number;
  patientId: number;
  patientName: string;
  patientNameAr?: string | null;
  doctorId: number;
  doctorName: string;
  doctorNameAr?: string | null;
  specialty: string;
  specialtyAr: string;
  departmentId: number;
  departmentName: string;
  departmentNameAr: string;
  appointmentDate: string;
  startTime: string;
  endTime: string;
  statusId: number;
  status: string;
  statusAr: string;
  notes?: string | null;
  createdByUserId: number;
  createdAt: string;
  updatedAt?: string | null;
}

export interface CreateAppointmentRequest {
  patientId?: number;
  doctorId: number;
  appointmentDate: string;
  startTime: string;
  endTime?: string;
  notes?: string;
}

export interface DepartmentDto {
  id: number;
  name: string;
  nameAr: string;
  description?: string | null;
}

export interface DoctorDto {
  id: number;
  fullName: string;
  fullNameAr?: string | null;
  specialty: string;
  specialtyAr: string;
  departmentId: number;
  departmentName: string;
  departmentNameAr: string;
}

export interface AppointmentStatusDto {
  id: number;
  name: string;
  nameAr: string;
}

export interface PatientListItemDto {
  id: number;
  fullName: string;
  fullNameAr?: string | null;
  phone?: string | null;
  nationalId?: string | null;
}

export interface CreateDepartmentRequest {
  name: string;
  nameAr: string;
  description?: string;
}

export interface CreateDoctorRequest {
  userName: string;
  email: string;
  password: string;
  fullName: string;
  fullNameAr?: string;
  departmentId: number;
  specialty: string;
  specialtyAr: string;
  licenseNumber: string;
}
