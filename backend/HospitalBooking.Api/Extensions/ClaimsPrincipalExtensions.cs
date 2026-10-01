using System.Security.Claims;
using HospitalBooking.Application.Exceptions;

namespace HospitalBooking.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(value, out var userId))
        {
            throw new AppException("Invalid token user id. / معرّف المستخدم في التوكن غير صالح.", 401);
        }

        return userId;
    }

    public static string GetRole(this ClaimsPrincipal user)
    {
        var role = user.FindFirstValue(ClaimTypes.Role);
        if (string.IsNullOrWhiteSpace(role))
        {
            throw new AppException("Role claim is missing. / الدور غير موجود في التوكن.", 401);
        }

        return role;
    }
}
