using HospitalBooking.Domain.Entities;

namespace HospitalBooking.Application.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) CreateToken(User user);
}
