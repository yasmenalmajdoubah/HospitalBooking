# Step 07–09 — Angular UI + seed + local run

**الحالة:** ✅ مكتمل  
**Commit:** `step-07: angular UI, catalog management, and local SQLite demo`

---

## ما تم

- Angular 19 app (`frontend/hospital-booking`) بستايل داكن عربي
- صفحات: Login / Appointments / Book / Manage (Admin)
- ربط مع API عبر proxy على `/api`
- لأن SQL LocalDB غير مثبت محلياً: Provider = `Sqlite` للتجربة
- Seed: أقسام + أطباء + مريض + admin

## تشغيل

```powershell
# API
cd backend
dotnet run --project HospitalBooking.Api --launch-profile http

# Frontend
cd frontend/hospital-booking
npm start
```

- UI: http://localhost:4200
- Swagger: http://localhost:5268/swagger

## حسابات تجريبية

| User | Password | Role |
|------|----------|------|
| admin | Admin@123 | Admin |
| dr.ahmad | Doctor@123 | Doctor |
| sara | Sara@123 | Patient |
