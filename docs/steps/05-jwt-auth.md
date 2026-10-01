# Step 05 — JWT Authentication + Roles

**التاريخ:** 2026-10-01  
**الحالة:** ✅ مكتمل  
**Commit:** `step-05: add JWT authentication and role-based login`

---

## الهدف

إضافة تسجيل الدخول والتسجيل عبر JWT، مع دعم الأدوار:

`Admin` / `Doctor` / `Receptionist` / `Patient`

---

## Endpoints الجديدة

| Method | URL | Auth | الوظيفة |
|--------|-----|------|---------|
| POST | `/api/auth/login` | لا | تسجيل الدخول |
| POST | `/api/auth/register-patient` | لا | تسجيل مريض جديد |
| GET | `/api/auth/me` | JWT مطلوب | بيانات المستخدم الحالي |

---

## حساب الأدمن الافتراضي (Seed)

يُنشأ تلقائياً عند تشغيل الـ API في Development **إذا كانت قاعدة البيانات متصلة**:

| الحقل | القيمة |
|------|--------|
| UserName | `admin` |
| Password | `Admin@123` |
| Email | `admin@hospitalbooking.local` |
| Role | Admin / مدير النظام |

> غيّر كلمة المرور بعد أول دخول في بيئة حقيقية.

---

## مثال Login

```http
POST http://localhost:5268/api/auth/login
Content-Type: application/json

{
  "userNameOrEmail": "admin",
  "password": "Admin@123"
}
```

### الاستجابة

```json
{
  "accessToken": "eyJhbGciOiJI...",
  "expiresAtUtc": "2026-10-01T...",
  "user": {
    "id": 1,
    "userName": "admin",
    "email": "admin@hospitalbooking.local",
    "fullName": "System Administrator",
    "fullNameAr": "مدير النظام",
    "role": "Admin",
    "roleAr": "مدير النظام"
  }
}
```

---

## مثال Register Patient

```http
POST http://localhost:5268/api/auth/register-patient
Content-Type: application/json

{
  "userName": "sara",
  "email": "sara@example.com",
  "password": "Sara@123",
  "fullName": "Sara Ahmad",
  "fullNameAr": "سارة أحمد",
  "phone": "0599000000",
  "nationalId": "123456789"
}
```

ينشئ:
1. سجل في `Users` بدور `Patient`
2. سجل في `Patients` مربوط بنفس المستخدم
3. يرجع JWT مباشرة

---

## إعدادات JWT (`appsettings.json`)

```json
"Jwt": {
  "Issuer": "HospitalBooking",
  "Audience": "HospitalBooking.Client",
  "Key": "HospitalBooking_Dev_Secret_Key_Change_Me_32+",
  "ExpiryMinutes": 120
}
```

| المفتاح | المعنى |
|---------|--------|
| Issuer | مصدر التوكن |
| Audience | الجمهور المستهدف (Angular لاحقاً) |
| Key | مفتاح التوقيع (32 حرف على الأقل) |
| ExpiryMinutes | مدة صلاحية التوكن بالدقائق |

---

## Claims داخل التوكن

| Claim | القيمة |
|-------|--------|
| `sub` / `NameIdentifier` | User Id |
| `unique_name` | UserName |
| `email` | Email |
| `role` | Admin / Doctor / Receptionist / Patient |
| `fullName` | الاسم |
| `roleAr` | اسم الدور بالعربي |

---

## الملفات المضافة

```
Application/
  DTOs/Auth/          LoginRequest, RegisterPatientRequest, AuthResponse, UserDto
  Interfaces/         IAuthService, IJwtTokenService, IPasswordHasherService
  Options/JwtSettings.cs
  Exceptions/AppException.cs

Infrastructure/Auth/
  AuthService.cs
  JwtTokenService.cs
  PasswordHasherService.cs

Infrastructure/Persistence/
  DbInitializer.cs    ← Migrate + seed admin

Api/Controllers/
  AuthController.cs
```

---

## Swagger

1. شغّل الـ API
2. افتح `/swagger`
3. نفّذ `POST /api/auth/login`
4. انسخ `accessToken`
5. اضغط **Authorize** والصق التوكن
6. جرّب `GET /api/auth/me`

---

## ملاحظة قاعدة البيانات

إذا LocalDB غير مثبت، الـ Login لن يعمل لأن Users غير موجودة بعد.

بعد تثبيت SQL Server LocalDB:

```powershell
cd C:\Users\SAA\Projects\HospitalBooking\backend
dotnet ef database update --project HospitalBooking.Infrastructure --startup-project HospitalBooking.Api
dotnet run --project HospitalBooking.Api --launch-profile http
```

عند التشغيل في Development يتم:
1. تطبيق Migrations تلقائياً
2. إنشاء مستخدم `admin` إن لم يكن موجوداً

---

## الخطوة التالية — Step 06

APIs الحجوزات (CRUD Appointments) مع صلاحيات حسب الدور.
