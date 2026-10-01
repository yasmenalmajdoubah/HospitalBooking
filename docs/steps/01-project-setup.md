# Step 01 — إعداد المشروع ورفعه على GitHub

**التاريخ:** 2026-10-01  
**الحالة:** ✅ مكتمل  
**Commit:** `step-01: project structure and documentation`

---

## الهدف من هذه الخطوة

إنشاء هيكل المشروع الفارغ، توثيق المعمارية، وأسماء قاعدة البيانات والجداول، ثم رفع كل شيء إلى GitHub كأول commit مستقل.

---

## القرارات المتفق عليها

| البند | القيمة |
|------|--------|
| اسم المشروع | `HospitalBooking` |
| مسار الحفظ | `C:\Users\SAA\Projects\HospitalBooking` |
| Backend | ASP.NET Core 8 |
| Frontend | Angular |
| Database | SQL Server LocalDB |
| اسم قاعدة البيانات | **`HospitalBookingDb`** |
| اللغات | عربي + إنجليزي |
| حساب GitHub | `yasmenalmajdoubah` |

---

## ما تم إنشاؤه في هذه الخطوة

```
HospitalBooking/
├── README.md                 ← شرح عام للمشروع (عربي/إنجليزي)
├── .gitignore                ← تجاهل bin/obj/node_modules/secrets
├── backend/                  ← مكان حلّ .NET (فارغ حالياً)
│   └── .gitkeep
├── frontend/                 ← مكان تطبيق Angular (فارغ حالياً)
│   └── .gitkeep
└── docs/
    └── steps/
        └── 01-project-setup.md   ← هذا الملف
```

---

## الأدوات التي تم تثبيتها على الجهاز

| الأداة | الإصدار / الملاحظات |
|--------|---------------------|
| .NET SDK | 8.0.425 |
| Node.js | LTS (مثبّت) |
| Git | موجود مسبقاً |
| GitHub CLI (`gh`) | مسجّل دخول |

---

## قاعدة البيانات — أسماء واضحة (للخطوات القادمة)

**Database name:** `HospitalBookingDb`

### الجداول المخططة

| اسم الجدول (EN) | الاسم بالعربي | الوظيفة |
|-----------------|---------------|---------|
| `Users` | المستخدمون | حسابات الدخول |
| `Roles` | الأدوار | Admin / Doctor / Receptionist / Patient |
| `Departments` | الأقسام | أقسام المستشفى |
| `Doctors` | الأطباء | بيانات الأطباء وربطهم بالقسم |
| `Patients` | المرضى | بيانات المرضى |
| `DoctorSchedules` | جداول الأطباء | أوقات الدوام والتوفر |
| `Appointments` | الحجوزات | مواعيد المرضى مع الأطباء |
| `AppointmentStatuses` | حالات الحجز | Pending / Confirmed / Cancelled / Completed |

> ملاحظة: الجداول لن تُنشأ إلا في **Step 04** عبر EF Core Migrations.

---

## طبقات الـ Backend (Clean Architecture)

| المشروع | المسؤولية |
|---------|-----------|
| `HospitalBooking.Domain` | Entities + Enums فقط (بدون dependencies) |
| `HospitalBooking.Application` | Business logic + DTOs + Interfaces |
| `HospitalBooking.Infrastructure` | EF Core + SQL Server + Repositories |
| `HospitalBooking.Api` | Controllers + JWT + Swagger + CORS |

---

## أدوار النظام (Roles)

| Role (EN) | بالعربي | الصلاحيات باختصار |
|-----------|---------|-------------------|
| `Admin` | مدير | إدارة كاملة |
| `Doctor` | طبيب | مواعيده وجدوله |
| `Receptionist` | استقبال | إنشاء وتعديل الحجوزات |
| `Patient` | مريض | حجز ومشاهدة مواعيده فقط |

---

## لماذا لم نكتب كود .NET أو Angular بعد؟

لأننا نرفع **كل خطوة لوحدها**:

1. هذه الخطوة = الهيكل + التوثيق فقط
2. الخطوة التالية (02) = إنشاء Solution .NET وتشغيل API فارغ
3. ثم Entities، ثم Database، ثم Auth... إلخ

هذا يسهّل المراجعة والتراجع وفهم كل تغيير.

---

## كيف تتأكد أن الخطوة نجحت؟

1. المجلد موجود: `C:\Users\SAA\Projects\HospitalBooking`
2. الريبو على GitHub يحتوي الملفات أعلاه
3. لا يوجد كود Backend/Frontend بعد (مجلدات فارغة مع `.gitkeep`)

---

## الخطوة التالية — Step 02

إنشاء حلّ .NET 8:

- `HospitalBooking.sln`
- المشاريع الأربعة (Api, Application, Domain, Infrastructure)
- تشغيل Swagger على `https://localhost:7xxx`
- Commit + Push منفصل بعنوان: `step-02: create .NET solution and empty API`
