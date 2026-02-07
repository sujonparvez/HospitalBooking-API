using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Interfaces.Auth; // Added
using HospitalBooking.Domain.Interfaces;
using HospitalBooking.Infrastructure.Auth; // Added
using HospitalBooking.Infrastructure.CQRS;
using HospitalBooking.Infrastructure.Persistence;
using HospitalBooking.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // CQRS Dispatcher
        services.AddScoped<IDispatcher, Dispatcher>();

        // Auth
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
