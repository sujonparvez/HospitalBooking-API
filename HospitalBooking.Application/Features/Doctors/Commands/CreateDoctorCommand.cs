namespace HospitalBooking.Application.Features.Doctors.Commands;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Doctors.DTOs;
using HospitalBooking.Application.Interfaces.Auth;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;
using FluentValidation;

public record CreateDoctorCommand(
    string FullName, 
    string Email, 
    string Password, 
    string PhoneNumber, 
    string Specialization, 
    int DepartmentId,
    int SlotDurationMinutes) : ICommand<DoctorDto>;

public class CreateDoctorCommandHandler : ICommandHandler<CreateDoctorCommand, DoctorDto>
{
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<Doctor> _doctorRepo;
    private readonly IRepository<Department> _deptRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    public CreateDoctorCommandHandler(
        IRepository<User> userRepo,
        IRepository<Doctor> doctorRepo,
        IRepository<Department> deptRepo,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepo = userRepo;
        _doctorRepo = doctorRepo;
        _deptRepo = deptRepo;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<DoctorDto> Handle(CreateDoctorCommand command, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken);
        try
        {
            // Check Department
            var dept = await _deptRepo.GetByIdAsync(command.DepartmentId);
            if (dept == null) throw new KeyNotFoundException("Department not found");

            // Check if email exists
            var existing = await _userRepo.FindAsync(u => u.Email == command.Email);
            if (existing.Any()) throw new InvalidOperationException("Email already exists");

            // Create User
            var user = new User
            {
                FullName = command.FullName,
                Email = command.Email,
                PasswordHash = _passwordHasher.HashPassword(command.Password),
                PhoneNumber = command.PhoneNumber,
                Role = UserRole.Doctor,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Admin",
                IsDeleted = false
            };

            await _userRepo.AddAsync(user);

            // Create Doctor Profile
            var doctor = new Doctor
            {
                UserId = user.Id,
                DepartmentId = command.DepartmentId,
                Specialization = command.Specialization,
                AppointmentSlotDurationMinutes = command.SlotDurationMinutes,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Admin",
                IsDeleted = false
            };

            await _doctorRepo.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            await transaction.CommitAsync(cancellationToken);

            return new DoctorDto(
            doctor.Id,
            user.FullName,
            user.Email,
            doctor.Specialization,
            dept.Id,
            dept.Name,
            doctor.AppointmentSlotDurationMinutes);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }       
    }
}

public class CreateDoctorCommandValidator : AbstractValidator<CreateDoctorCommand>
{
    public CreateDoctorCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.DepartmentId).GreaterThan(0);
        RuleFor(x => x.SlotDurationMinutes).InclusiveBetween(10, 60);
    }
}
