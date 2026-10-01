# Database Design Overview / نظرة عامة على قاعدة البيانات

> هذا الملف توثيق تخطيطي فقط. التنفيذ الفعلي في **Step 04**.

## Database

| Property | Value |
|----------|-------|
| Name | `HospitalBookingDb` |
| Engine | SQL Server (LocalDB for development) |
| Connection (planned) | `Server=(localdb)\\mssqllocaldb;Database=HospitalBookingDb;Trusted_Connection=True;TrustServerCertificate=True` |
| ORM | Entity Framework Core 8 |

---

## Tables / الجداول

### 1. Roles
| Column | Type | Notes |
|--------|------|-------|
| Id | int PK | |
| Name | nvarchar(50) | Admin, Doctor, Receptionist, Patient |
| NameAr | nvarchar(50) | مدير، طبيب، استقبال، مريض |

### 2. Users
| Column | Type | Notes |
|--------|------|-------|
| Id | int PK | |
| UserName | nvarchar(100) | unique |
| Email | nvarchar(200) | unique |
| PasswordHash | nvarchar(max) | |
| FullName | nvarchar(200) | |
| FullNameAr | nvarchar(200) | nullable |
| Phone | nvarchar(30) | nullable |
| RoleId | int FK → Roles | |
| IsActive | bit | default 1 |
| CreatedAt | datetime2 | |

### 3. Departments
| Column | Type | Notes |
|--------|------|-------|
| Id | int PK | |
| Name | nvarchar(150) | e.g. Cardiology |
| NameAr | nvarchar(150) | e.g. القلب |
| Description | nvarchar(500) | nullable |
| IsActive | bit | |

### 4. Doctors
| Column | Type | Notes |
|--------|------|-------|
| Id | int PK | |
| UserId | int FK → Users | login account |
| DepartmentId | int FK → Departments | |
| Specialty | nvarchar(150) | |
| SpecialtyAr | nvarchar(150) | |
| LicenseNumber | nvarchar(50) | |

### 5. Patients
| Column | Type | Notes |
|--------|------|-------|
| Id | int PK | |
| UserId | int FK → Users | nullable if walk-in |
| NationalId | nvarchar(50) | nullable |
| DateOfBirth | date | nullable |
| Gender | nvarchar(20) | |

### 6. DoctorSchedules
| Column | Type | Notes |
|--------|------|-------|
| Id | int PK | |
| DoctorId | int FK → Doctors | |
| DayOfWeek | int | 0=Sunday … 6=Saturday |
| StartTime | time | |
| EndTime | time | |
| SlotDurationMinutes | int | e.g. 30 |
| IsActive | bit | |

### 7. AppointmentStatuses
| Column | Type | Notes |
|--------|------|-------|
| Id | int PK | |
| Name | nvarchar(50) | Pending, Confirmed, Cancelled, Completed |
| NameAr | nvarchar(50) | قيد الانتظار، مؤكد، ملغي، مكتمل |

### 8. Appointments
| Column | Type | Notes |
|--------|------|-------|
| Id | int PK | |
| PatientId | int FK → Patients | |
| DoctorId | int FK → Doctors | |
| AppointmentDate | date | |
| StartTime | time | |
| EndTime | time | |
| StatusId | int FK → AppointmentStatuses | |
| Notes | nvarchar(1000) | nullable |
| CreatedByUserId | int FK → Users | |
| CreatedAt | datetime2 | |
| UpdatedAt | datetime2 | nullable |

---

## Entity Relationship (simplified)

```
Roles 1──* Users
Users 1──0..1 Doctors
Users 1──0..1 Patients
Departments 1──* Doctors
Doctors 1──* DoctorSchedules
Doctors 1──* Appointments
Patients 1──* Appointments
AppointmentStatuses 1──* Appointments
```
