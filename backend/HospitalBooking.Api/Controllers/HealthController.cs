using Microsoft.AspNetCore.Mvc;

namespace HospitalBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Health check — confirms the API is running.
    /// فحص صحة الـ API — للتأكد أن السيرفر يعمل.
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "Healthy",
            statusAr = "يعمل بشكل سليم",
            service = "HospitalBooking.Api",
            database = "HospitalBookingDb (not connected yet — Step 04)",
            utc = DateTime.UtcNow
        });
    }
}
