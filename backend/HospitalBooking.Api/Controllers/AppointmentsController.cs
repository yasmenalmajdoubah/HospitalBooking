using HospitalBooking.Api.Extensions;
using HospitalBooking.Application.DTOs.Appointments;
using HospitalBooking.Application.Exceptions;
using HospitalBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalBooking.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    /// <summary>
    /// List appointments (scoped by role) / قائمة الحجوزات حسب الدور
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AppointmentDto>>> Get(
        [FromQuery] AppointmentFilter filter,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _appointmentService.GetAsync(
                filter,
                User.GetUserId(),
                User.GetRole(),
                cancellationToken);
            return Ok(result);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get appointment by id / تفاصيل حجز
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppointmentDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _appointmentService.GetByIdAsync(
                id,
                User.GetUserId(),
                User.GetRole(),
                cancellationToken);

            return result is null
                ? NotFound(new { message = "Appointment not found. / الحجز غير موجود." })
                : Ok(result);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Create appointment / إنشاء حجز
    /// Patient: books for self. Admin/Receptionist: must send patientId.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Receptionist,Patient")]
    public async Task<ActionResult<AppointmentDto>> Create(
        [FromBody] CreateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _appointmentService.CreateAsync(
                request,
                User.GetUserId(),
                User.GetRole(),
                cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update appointment date/time/notes / تعديل موعد الحجز
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<ActionResult<AppointmentDto>> Update(
        int id,
        [FromBody] UpdateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _appointmentService.UpdateAsync(
                id,
                request,
                User.GetUserId(),
                User.GetRole(),
                cancellationToken);
            return Ok(result);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update appointment status / تغيير حالة الحجز
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin,Receptionist,Doctor,Patient")]
    public async Task<ActionResult<AppointmentDto>> UpdateStatus(
        int id,
        [FromBody] UpdateAppointmentStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _appointmentService.UpdateStatusAsync(
                id,
                request,
                User.GetUserId(),
                User.GetRole(),
                cancellationToken);
            return Ok(result);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cancel appointment / إلغاء حجز
    /// </summary>
    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles = "Admin,Receptionist,Doctor,Patient")]
    public async Task<ActionResult<AppointmentDto>> Cancel(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _appointmentService.CancelAsync(
                id,
                User.GetUserId(),
                User.GetRole(),
                cancellationToken);
            return Ok(result);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }
}
