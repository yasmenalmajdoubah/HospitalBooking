# Step 04 — EF Core + HospitalBookingDb + Migration

**التاريخ:** 2026-10-01  
**الحالة:** ✅ مكتمل (الكود + Migration) — تطبيق القاعدة يحتاج SQL Server على الجهاز  
**Commit:** `step-04: add EF Core DbContext and InitialCreate migration`

---

## الهدف

ربط Domain Entities مع SQL Server عبر EF Core، إنشاء `HospitalBookingDbContext`، وعمل Migration أولى باسم **`InitialCreate`** لإنشاء قاعدة **`HospitalBookingDb`**.

---

## ما تم إنشاؤه

```
HospitalBooking.Infrastructure/
├── DependencyInjection.cs          ← تسجيل DbContext
└── Persistence/
    ├── HospitalBookingDbContext.cs
    ├── HospitalBookingDbContextFactory.cs   ← لـ dotnet ef
    ├── DbSeed.cs                            ← Roles + AppointmentStatuses
    ├── Configurations/                      ← Fluent API لكل جدول
    │   ├── RoleConfiguration.cs
    │   ├── UserConfiguration.cs
    │   ├── DepartmentConfiguration.cs
    │   ├── DoctorConfiguration.cs
    │   ├── PatientConfiguration.cs
    │   ├── DoctorScheduleConfiguration.cs
    │   ├── AppointmentStatusConfiguration.cs
    │   └── AppointmentConfiguration.cs
    └── Migrations/
        └── *_InitialCreate.cs

docs/sql/
└── 01-InitialCreate.sql            ← سكربت SQL كامل (idempotent)
```

---

## اسم قاعدة البيانات والاتصال

| البند | القيمة |
|------|--------|
| Database | **`HospitalBookingDb`** |
| Connection key | `ConnectionStrings:HospitalBookingDb` |
| Server (dev) | `(localdb)\mssqllocaldb` |
| Connection string | في `HospitalBooking.Api/appsettings.json` |

```
Server=(localdb)\mssqllocaldb;Database=HospitalBookingDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
```

---

## الجداول التي تنشئها Migration

| الجدول | Seed؟ |
|--------|-------|
| `Roles` | ✅ Admin / Doctor / Receptionist / Patient |
| `AppointmentStatuses` | ✅ Pending / Confirmed / Cancelled / Completed |
| `Users` | لا |
| `Departments` | لا |
| `Doctors` | لا |
| `Patients` | لا |
| `DoctorSchedules` | لا |
| `Appointments` | لا |
| `__EFMigrationsHistory` | تلقائي من EF |

---

## أوامر مهمة

```powershell
cd C:\Users\SAA\Projects\HospitalBooking\backend

# إنشاء Migration جديدة (لاحقاً عند تغيير الموديل)
dotnet ef migrations add <Name> --project HospitalBooking.Infrastructure --startup-project HospitalBooking.Api --output-dir Persistence/Migrations

# تطبيق Migration وإنشاء HospitalBookingDb
dotnet ef database update --project HospitalBooking.Infrastructure --startup-project HospitalBooking.Api
```

---

## ملاحظة مهمة عن الجهاز الحالي

عند تنفيذ Step 04، الجهاز **لا يحتوي** على SQL Server LocalDB / Express مثبتاً، وروابط winget الرسمية رجعت 404.

لذلك:
1. الكود + Migration + سكربت SQL **جاهزين ومرفوعين**
2. لإنشاء القاعدة فعلياً على جهازك، ثبّت أحد التالي ثم نفّذ `dotnet ef database update`:

### خيار أ — SQL Server Express LocalDB (موصى به للتطوير)
- ثبّت من Visual Studio Installer → Individual components → **SQL Server Express LocalDB**
- أو من صفحة تحميل SQL Server Express واختر LocalDB

### خيار ب — SQL Server Express كامل
- ثبّت Express ثم عدّل الـ connection string إلى:
  `Server=.\\SQLEXPRESS;Database=HospitalBookingDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true`

بعد التثبيت:
```powershell
dotnet ef database update --project HospitalBooking.Infrastructure --startup-project HospitalBooking.Api
```

ثم افتح: `http://localhost:5268/api/health`  
يجب أن ترى `"connected": true` و`"name": "HospitalBookingDb"`.

---

## Health Endpoint بعد هذه الخطوة

`GET /api/health` يعرض:
- هل الـ API يعمل
- هل متصل بـ `HospitalBookingDb`
- الـ migrations المطبّقة / المعلّقة

---

## الخطوة التالية — Step 05

JWT Authentication + تسجيل الدخول حسب الأدوار (Admin / Doctor / Receptionist / Patient).
