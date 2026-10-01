# Step 06 — Appointments APIs (CRUD + Roles)

**التاريخ:** 2026-10-01  
**الحالة:** ✅ مكتمل  
**Commit:** `step-06: add appointments APIs with role-based access`

---

## الهدف

بناء APIs الحجوزات مع صلاحيات حسب الدور، بالإضافة لـ Catalog بسيط (أقسام / أطباء / حالات الحجز) لدعم شاشة الحجز لاحقاً.

---

## Appointments Endpoints

| Method | URL | الأدوار | الوظيفة |
|--------|-----|---------|---------|
| GET | `/api/appointments` | الكل (مع فلترة حسب الدور) | قائمة الحجوزات |
| GET | `/api/appointments/{id}` | الكل (ضمن نطاقه) | تفاصيل حجز |
| POST | `/api/appointments` | Admin, Receptionist, Patient | إنشاء حجز |
| PUT | `/api/appointments/{id}` | Admin, Receptionist | تعديل موعد/ملاحظات |
| PATCH | `/api/appointments/{id}/status` | Admin, Receptionist, Doctor, Patient* | تغيير الحالة |
| POST | `/api/appointments/{id}/cancel` | Admin, Receptionist, Doctor, Patient | إلغاء |

\* Patient يقدر يغيّر الحالة إلى **Cancelled** فقط.

---

## Catalog Endpoints

| Method | URL | الوظيفة |
|--------|-----|---------|
| GET | `/api/catalog/departments` | الأقسام النشطة |
| GET | `/api/catalog/doctors?departmentId=` | الأطباء (فلتر اختياري بالقسم) |
| GET | `/api/catalog/appointment-statuses` | حالات الحجز |

---

## قواعد الصلاحيات (مهم)

| الدور | ماذا يرى؟ | ماذا يفعل؟ |
|-------|-----------|------------|
| **Admin** | كل الحجوزات | إنشاء / تعديل / تغيير حالة / إلغاء |
| **Receptionist** | كل الحجوزات | إنشاء / تعديل / تغيير حالة / إلغاء |
| **Doctor** | حجوزاته فقط | تأكيد / إكمال / إلغاء |
| **Patient** | حجوزاته فقط | إنشاء لنفسه / إلغاء |

---

## Query Filters لـ GET /api/appointments

| Param | مثال | ملاحظة |
|-------|------|--------|
| `fromDate` | `2026-10-01` | |
| `toDate` | `2026-10-31` | |
| `doctorId` | `1` | للـ Staff فقط |
| `patientId` | `2` | للـ Staff فقط |
| `statusId` | `1` | Pending=1 ... |
| `departmentId` | `1` | للـ Staff فقط |

---

## حالات الحجز (`AppointmentStatuses`)

| Id | EN | AR |
|----|----|----|
| 1 | Pending | قيد الانتظار |
| 2 | Confirmed | مؤكد |
| 3 | Cancelled | ملغي |
| 4 | Completed | مكتمل |

---

## مثال إنشاء حجز (مريض)

```http
POST /api/appointments
Authorization: Bearer {token}
Content-Type: application/json

{
  "doctorId": 1,
  "appointmentDate": "2026-10-10",
  "startTime": "09:00:00",
  "notes": "First visit"
}
```

> المريض: لا يرسل `patientId` (يُؤخذ من حسابه تلقائياً).

## مثال إنشاء حجز (استقبال)

```json
{
  "patientId": 2,
  "doctorId": 1,
  "appointmentDate": "2026-10-10",
  "startTime": "09:30:00",
  "endTime": "10:00:00",
  "notes": "Follow up"
}
```

---

## حماية التعارض (Conflict)

عند الإنشاء/التعديل يتم رفض الطلب إذا:
- الطبيب لديه موعد متداخل غير ملغي
- أو المريض لديه موعد متداخل غير ملغي

يرجع HTTP **409**.

---

## الملفات الجديدة

```
Application/DTOs/Appointments/*
Application/DTOs/Catalog/*
Application/Interfaces/IAppointmentService.cs
Application/Interfaces/ICatalogService.cs

Infrastructure/Services/AppointmentService.cs
Infrastructure/Services/CatalogService.cs

Api/Controllers/AppointmentsController.cs
Api/Controllers/CatalogController.cs
Api/Extensions/ClaimsPrincipalExtensions.cs
```

---

## كيف تختبر (بعد توفر SQL LocalDB)

1. Login كـ `admin`
2. أنشئ قسم/طبيب/مريض (حالياً يدوياً في DB أو عبر seed لاحق)
3. Login كمريض → `POST /api/appointments`
4. Login كطبيب → `PATCH /api/appointments/{id}/status` بـ Confirmed

---

## الخطوة التالية — Step 07

إنشاء مشروع Angular + ربطه مع الـ API (Login + قائمة الحجوزات).
