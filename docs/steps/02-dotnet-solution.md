# Step 02 — إنشاء حلّ .NET 8 و API فارغ

**التاريخ:** 2026-10-01  
**الحالة:** ✅ مكتمل  
**Commit:** `step-02: create .NET solution and empty API`

---

## الهدف

بناء Backend بـ Clean Architecture، تشغيل Web API مع Swagger، وتأكيد أن السيرفر يشتغل عبر endpoint الصحة — بدون Entities أو Database بعد.

---

## ما تم إنشاؤه

```
backend/
├── HospitalBooking.sln
├── HospitalBooking.Api/
│   ├── Controllers/HealthController.cs
│   ├── Program.cs
│   ├── appsettings.json          ← ConnectionStrings:HospitalBookingDb
│   └── Properties/launchSettings.json
├── HospitalBooking.Application/
│   ├── DependencyInjection.cs
│   ├── DTOs/
│   ├── Interfaces/
│   └── Services/
├── HospitalBooking.Domain/
│   ├── DomainAssembly.cs
│   ├── Entities/                 ← فارغ حتى Step 03
│   └── Enums/
└── HospitalBooking.Infrastructure/
    ├── DependencyInjection.cs
    ├── Persistence/              ← فارغ حتى Step 04
    └── Repositories/
```

---

## علاقات المشاريع (References)

```
Api ──► Application
Api ──► Infrastructure
Application ──► Domain
Infrastructure ──► Application
Infrastructure ──► Domain
```

**قاعدة مهمة:** Domain لا يشير لأي مشروع آخر.

---

## إعدادات مهمة

### اسم قاعدة البيانات (Connection String جاهز، الربط لاحقاً)

في `appsettings.json`:

```json
"ConnectionStrings": {
  "HospitalBookingDb": "Server=(localdb)\\mssqllocaldb;Database=HospitalBookingDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

| المفتاح | القيمة |
|---------|--------|
| Connection string name | `HospitalBookingDb` |
| Database name | `HospitalBookingDb` |
| Server (dev) | `(localdb)\mssqllocaldb` |

> الربط الفعلي مع EF Core سيكون في **Step 04**.

### المنافذ (Ports)

| Profile | URL |
|---------|-----|
| http | `http://localhost:5268` |
| https | `https://localhost:7158` |
| Swagger | `http://localhost:5268/swagger` |
| Health | `http://localhost:5268/api/health` |

### CORS

مسموح لـ Angular لاحقاً من:

- `http://localhost:4200`
- `https://localhost:4200`

---

## Health Endpoint — مثال الاستجابة

`GET /api/health`

```json
{
  "status": "Healthy",
  "statusAr": "يعمل بشكل سليم",
  "service": "HospitalBooking.Api",
  "database": "HospitalBookingDb (not connected yet — Step 04)",
  "utc": "2026-10-01T..."
}
```

---

## كيف تشغّل الـ API محلياً

```powershell
cd C:\Users\SAA\Projects\HospitalBooking\backend
dotnet run --project HospitalBooking.Api --launch-profile http
```

ثم افتح المتصفح على: http://localhost:5268/swagger

---

## كيف تتحقق أن الخطوة نجحت؟

1. `dotnet build backend/HospitalBooking.sln` → **0 errors**
2. السيرفر يستمع على `5268`
3. `/api/health` يرجع `Healthy`
4. Swagger يفتح بدون أخطاء

---

## ما لم نفعله عمداً (للخطوات القادمة)

| مؤجّل | الخطوة |
|-------|--------|
| Entities / Enums | Step 03 |
| EF Core + Migration + إنشاء DB | Step 04 |
| JWT + Roles | Step 05 |
| Appointments CRUD | Step 06 |

---

## الخطوة التالية — Step 03

إنشاء Domain Entities بأسماء الجداول النهائية:

`User`, `Role`, `Department`, `Doctor`, `Patient`, `DoctorSchedule`, `Appointment`, `AppointmentStatus`
