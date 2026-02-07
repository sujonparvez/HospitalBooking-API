namespace HospitalBooking.Infrastructure.Persistence;

using HospitalBooking.Application.Interfaces.Auth;
using HospitalBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        // Ensure Database Created
        // await context.Database.EnsureCreatedAsync(); // EnsureCreated doesn't work well with Migrations. MigrateAsync is better.

        // Check if Admin exists
        if (!await context.Users.AnyAsync(u => u.Email == "admin@hospital.com"))
        {
            var admin = new User
            {
                FullName = "System Admin",
                Email = "admin@hospital.com",
                PasswordHash = passwordHasher.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                PhoneNumber = "1234567890",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                IsDeleted = false
            };
            
            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
