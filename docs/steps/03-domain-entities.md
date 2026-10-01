# Step 03 — Domain Entities

**التاريخ:** 2026-10-01  
**الحالة:** ✅ مكتمل  
**Commit:** `step-03: add domain entities and enums`

---

## الهدف

تعريف كيانات النظام (Entities) والـ Enums في طبقة Domain — بدون EF Core وبدون إنشاء قاعدة البيانات بعد.

هذه الطبقة تطابق جداول **`HospitalBookingDb`**.

---

## الملفات المضافة

```
HospitalBooking.Domain/
├── Enums/
│   ├── SystemRole.cs              # Admin=1, Doctor=2, Receptionist=3, Patient=4
│   ├── AppointmentStatusCode.cs   # Pending=1, Confirmed=2, Cancelled=3, Completed=4
│   └── Gender.cs                  # Male, Female, Other
└── Entities/
    ├── Role.cs                    → جدول Roles
    ├── User.cs                    → جدول Users
    ├── Department.cs              → جدول Departments
    ├── Doctor.cs                  → جدول Doctors
    ├── Patient.cs                 → جدول Patients
    ├── DoctorSchedule.cs          → جدول DoctorSchedules
    ├── AppointmentStatus.cs       → جدول AppointmentStatuses
    └── Appointment.cs             → جدول Appointments
```

---

## ربط Entity → اسم الجدول في SQL

| Entity (C#) | Table (SQL) | ملاحظات |
|-------------|-------------|---------|
| `Role` | `Roles` | Lookup للأدوار + NameAr |
| `User` | `Users` | حساب الدخول + RoleId |
| `Department` | `Departments` | قسم المستشفى عربي/إنجليزي |
| `Doctor` | `Doctors` | مرتبط بـ User و Department |
| `Patient` | `Patients` | UserId اختياري (مريض بدون حساب) |
| `DoctorSchedule` | `DoctorSchedules` | يوم الأسبوع + أوقات الدوام |
| `AppointmentStatus` | `AppointmentStatuses` | Lookup لحالة الحجز |
| `Appointment` | `Appointments` | الحجز الفعلي |

**اسم قاعدة البيانات:** `HospitalBookingDb` (لم تُنشأ بعد — Step 04)

---

## الـ Enums ولماذا موجودة مع جداول Lookup؟

| Enum | يطابق |
|------|--------|
| `SystemRole` | قيم `Roles.Id` عند الـ Seed |
| `AppointmentStatusCode` | قيم `AppointmentStatuses.Id` عند الـ Seed |
| `Gender` | يُخزَّن مع المريض (Male/Female/Other) |

الجداول (`Roles`, `AppointmentStatuses`) تبقى لأن الواجهة تحتاج `Name` + `NameAr`.

---

## العلاقات (Navigation Properties)

```
Role 1 ─── * User
User 1 ─── 0..1 Doctor
User 1 ─── 0..1 Patient
Department 1 ─── * Doctor
Doctor 1 ─── * DoctorSchedule
Doctor 1 ─── * Appointment
Patient 1 ─── * Appointment
AppointmentStatus 1 ─── * Appointment
User 1 ─── * Appointment (CreatedByUser)
```

---

## حقول ثنائية اللغة (EN / AR)

| Entity | English | Arabic |
|--------|---------|--------|
| Role | Name | NameAr |
| User | FullName | FullNameAr |
| Department | Name | NameAr |
| Doctor | Specialty | SpecialtyAr |
| AppointmentStatus | Name | NameAr |

---

## ما لم نفعله عمداً

| مؤجّل | الخطوة |
|-------|--------|
| `DbContext` + Fluent API | Step 04 |
| Migration + إنشاء `HospitalBookingDb` | Step 04 |
| Seed للأدوار والحالات | Step 04 / 09 |
| JWT Auth | Step 05 |

---

## كيف تتحقق؟

```powershell
cd C:\Users\SAA\Projects\HospitalBooking\backend
dotnet build HospitalBooking.sln
```

يجب أن ينجح البناء بدون أخطاء.

---

## الخطوة التالية — Step 04

- إضافة EF Core packages
- إنشاء `HospitalBookingDbContext`
- Mapping الجداول
- Migration أولى باسم مثلاً `InitialCreate`
- تطبيقها لإنشاء قاعدة **`HospitalBookingDb`**
