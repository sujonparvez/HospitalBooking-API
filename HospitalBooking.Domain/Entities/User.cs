namespace HospitalBooking.Domain.Entities;

using HospitalBooking.Domain.Common;

public enum UserRole
{
    Admin = 1,
    Doctor = 2,
    Patient = 3
}

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty; // Storing PBKDF2 hash
    public string PhoneNumber { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
