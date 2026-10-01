using HospitalBooking.Application.DTOs.Auth;
using HospitalBooking.Application.Exceptions;
using HospitalBooking.Application.Interfaces;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Enums;
using HospitalBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HospitalBooking.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly HospitalBookingDbContext _dbContext;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        HospitalBookingDbContext dbContext,
        IPasswordHasherService passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var login = request.UserNameOrEmail.Trim();

            var user = await _dbContext.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(
                    x => x.UserName == login || x.Email == login,
                    cancellationToken);

            if (user is null || !_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
            {
                throw new AppException("Invalid username/email or password. / اسم المستخدم أو كلمة المرور غير صحيحة.", 401);
            }

            if (!user.IsActive)
            {
                throw new AppException("This account is inactive. / هذا الحساب غير مفعّل.", 403);
            }

            return CreateAuthResponse(user);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new AppException(
                "Database connection failed. / فشل الاتصال بقاعدة البيانات. تأكد أن الـ API شغال وأن Database:Provider مضبوط.",
                503);
        }
    }

    public async Task<AuthResponse> RegisterPatientAsync(
        RegisterPatientRequest request,
        CancellationToken cancellationToken = default)
    {
        var userName = request.UserName.Trim();
        var email = request.Email.Trim().ToLowerInvariant();

        var exists = await _dbContext.Users.AnyAsync(
            x => x.UserName == userName || x.Email == email,
            cancellationToken);

        if (exists)
        {
            throw new AppException("Username or email already exists. / اسم المستخدم أو البريد مستخدم مسبقاً.", 409);
        }

        var user = new User
        {
            UserName = userName,
            Email = email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            FullName = request.FullName.Trim(),
            FullNameAr = string.IsNullOrWhiteSpace(request.FullNameAr) ? null : request.FullNameAr.Trim(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            RoleId = (int)SystemRole.Patient,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Patient = new Patient
            {
                NationalId = string.IsNullOrWhiteSpace(request.NationalId) ? null : request.NationalId.Trim()
            }
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.Entry(user).Reference(x => x.Role).LoadAsync(cancellationToken);

        return CreateAuthResponse(user);
    }

    public async Task<UserDto?> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        return user is null ? null : MapUser(user);
    }

    private AuthResponse CreateAuthResponse(User user)
    {
        var (token, expiresAt) = _jwtTokenService.CreateToken(user);

        return new AuthResponse
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAt,
            User = MapUser(user)
        };
    }

    private static UserDto MapUser(User user) => new()
    {
        Id = user.Id,
        UserName = user.UserName,
        Email = user.Email,
        FullName = user.FullName,
        FullNameAr = user.FullNameAr,
        Role = user.Role.Name,
        RoleAr = user.Role.NameAr
    };
}
