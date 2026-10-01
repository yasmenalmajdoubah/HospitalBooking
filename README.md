# HospitalBooking / نظام حجوزات المستشفى

نظام حجوزات مواعيد مستشفى مبني بـ **ASP.NET Core 8** + **Angular** + **SQL Server**.

Hospital appointment booking system built with **ASP.NET Core 8** + **Angular** + **SQL Server**.

---

## Tech Stack / التقنيات

| Layer | Technology |
|-------|------------|
| Backend | ASP.NET Core 8 Web API |
| Frontend | Angular (latest LTS) |
| Database | SQL Server LocalDB → `HospitalBookingDb` |
| Auth | JWT + Roles |
| Languages | Arabic + English (RTL/LTR) |

---

## Solution Structure / هيكل المشروع

```
HospitalBooking/
├── backend/                          # .NET solution
│   ├── HospitalBooking.Api/          # Controllers, JWT, Swagger
│   ├── HospitalBooking.Application/  # Services, DTOs, Validation
│   ├── HospitalBooking.Domain/       # Entities, Enums
│   └── HospitalBooking.Infrastructure/ # EF Core, SQL Server
├── frontend/                         # Angular app
│   └── hospital-booking/
└── docs/
    └── steps/                        # شرح كل خطوة بالتفصيل
```

---

## Database Name / اسم قاعدة البيانات

**`HospitalBookingDb`**

### Planned Tables / الجداول المخططة

| Table | Arabic | Purpose |
|-------|--------|---------|
| `Users` | المستخدمون | System accounts |
| `Roles` | الأدوار | Admin, Doctor, Receptionist, Patient |
| `Departments` | الأقسام | Cardiology, Orthopedics, ... |
| `Doctors` | الأطباء | Doctor profiles |
| `Patients` | المرضى | Patient profiles |
| `DoctorSchedules` | جداول الأطباء | Working hours / availability |
| `Appointments` | الحجوزات | Booked appointments |
| `AppointmentStatuses` | حالات الحجز | Pending, Confirmed, Cancelled, Completed |

---

## Roles / الأدوار

| Role | Arabic | Access |
|------|--------|--------|
| `Admin` | مدير النظام | Full system management |
| `Doctor` | طبيب | View schedule & appointments |
| `Receptionist` | موظف استقبال | Create/manage appointments |
| `Patient` | مريض | Book & view own appointments |

---

## Step-by-step Progress / تقدّم الخطوات

| Step | Description | Status |
|------|-------------|--------|
| 01 | Project structure + GitHub | ✅ Done |
| 02 | .NET solution (empty API) | ⏳ Next |
| 03 | Domain entities | ⏳ Pending |
| 04 | EF Core + HospitalBookingDb + Migration | ⏳ Pending |
| 05 | JWT Auth + Roles | ⏳ Pending |
| 06 | Appointments APIs | ⏳ Pending |
| 07 | Angular project + API link | ⏳ Pending |
| 08 | UI screens (Login, Departments, Booking) | ⏳ Pending |
| 09 | Seed data + polish | ⏳ Pending |

Detailed docs for each step: [`docs/steps/`](docs/steps/)

---

## Prerequisites / المتطلبات

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js LTS](https://nodejs.org/)
- [Git](https://git-scm.com/)
- SQL Server LocalDB (comes with Visual Studio / SQL Server Express)

---

## Getting Started (later steps)

```bash
# Backend
cd backend
dotnet run --project HospitalBooking.Api

# Frontend
cd frontend/hospital-booking
npm start
```

---

## License

Private / Educational project.
