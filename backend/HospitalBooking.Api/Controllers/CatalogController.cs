using HospitalBooking.Application.DTOs.Catalog;
using HospitalBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalBooking.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _catalogService;

    public CatalogController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    /// <summary>
    /// Active departments / الأقسام النشطة
    /// </summary>
    [HttpGet("departments")]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetDepartments(CancellationToken cancellationToken)
    {
        return Ok(await _catalogService.GetDepartmentsAsync(cancellationToken));
    }

    /// <summary>
    /// Active doctors (optional department filter) / الأطباء
    /// </summary>
    [HttpGet("doctors")]
    public async Task<ActionResult<IReadOnlyList<DoctorDto>>> GetDoctors(
        [FromQuery] int? departmentId,
        CancellationToken cancellationToken)
    {
        return Ok(await _catalogService.GetDoctorsAsync(departmentId, cancellationToken));
    }

    /// <summary>
    /// Appointment statuses / حالات الحجز
    /// </summary>
    [HttpGet("appointment-statuses")]
    public async Task<ActionResult<IReadOnlyList<AppointmentStatusDto>>> GetAppointmentStatuses(
        CancellationToken cancellationToken)
    {
        return Ok(await _catalogService.GetAppointmentStatusesAsync(cancellationToken));
    }
}
