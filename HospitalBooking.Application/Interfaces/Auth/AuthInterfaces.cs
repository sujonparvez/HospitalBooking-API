namespace HospitalBooking.Application.Interfaces.Auth;

using HospitalBooking.Domain.Entities;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
