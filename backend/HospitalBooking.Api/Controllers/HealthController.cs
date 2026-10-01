using HospitalBooking.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly HospitalBookingDbContext _dbContext;

    public HealthController(HospitalBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Health check — API + database connectivity.
    /// فحص صحة الـ API واتصال قاعدة البيانات.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var connected = false;
        IEnumerable<string> applied = Array.Empty<string>();
        IEnumerable<string> pending = Array.Empty<string>();
        string? dbError = null;

        try
        {
            connected = await _dbContext.Database.CanConnectAsync(cancellationToken);
            if (connected)
            {
                applied = await _dbContext.Database.GetAppliedMigrationsAsync(cancellationToken);
                pending = await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
            }
            else
            {
                pending = await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            dbError = ex.Message;
            pending = _dbContext.Database.GetMigrations();
        }

        return Ok(new
        {
            status = connected ? "Healthy" : "Degraded",
            statusAr = connected ? "يعمل بشكل سليم" : "مشكلة في الاتصال بقاعدة البيانات",
            service = "HospitalBooking.Api",
            database = new
            {
                name = "HospitalBookingDb",
                connected,
                appliedMigrations = applied,
                pendingMigrations = pending,
                error = dbError
            },
            utc = DateTime.UtcNow
        });
    }
}
