using HospitalBooking.Application.DTOs.Catalog;
using HospitalBooking.Application.Exceptions;
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

    [HttpGet("departments")]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetDepartments(CancellationToken cancellationToken)
    {
        return Ok(await _catalogService.GetDepartmentsAsync(cancellationToken));
    }

    [HttpPost("departments")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment(
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _catalogService.CreateDepartmentAsync(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpGet("doctors")]
    public async Task<ActionResult<IReadOnlyList<DoctorDto>>> GetDoctors(
        [FromQuery] int? departmentId,
        CancellationToken cancellationToken)
    {
        return Ok(await _catalogService.GetDoctorsAsync(departmentId, cancellationToken));
    }

    [HttpPost("doctors")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DoctorDto>> CreateDoctor(
        [FromBody] CreateDoctorRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _catalogService.CreateDoctorAsync(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpGet("patients")]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<ActionResult<IReadOnlyList<PatientListItemDto>>> GetPatients(CancellationToken cancellationToken)
    {
        return Ok(await _catalogService.GetPatientsAsync(cancellationToken));
    }

    [HttpGet("appointment-statuses")]
    public async Task<ActionResult<IReadOnlyList<AppointmentStatusDto>>> GetAppointmentStatuses(
        CancellationToken cancellationToken)
    {
        return Ok(await _catalogService.GetAppointmentStatusesAsync(cancellationToken));
    }
}
