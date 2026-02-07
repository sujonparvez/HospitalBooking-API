namespace HospitalBooking.Application.Features.Doctors.Commands;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;
using FluentValidation;

public record UpdateDoctorCommand(
    int Id,
    string FullName, 
    string Email, 
    string Specialization, 
    int DepartmentId,
    int SlotDurationMinutes) : ICommand;

public class UpdateDoctorCommandHandler : ICommandHandler<UpdateDoctorCommand>
{
    private readonly IDoctorRepository _doctorRepo;
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<Department> _deptRepo;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateDoctorCommandHandler(
        IDoctorRepository doctorRepo, 
        IRepository<User> userRepo,
        IRepository<Department> deptRepo,
        IUnitOfWork unitOfWork)
    {
        _doctorRepo = doctorRepo;
        _userRepo = userRepo;
        _deptRepo = deptRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateDoctorCommand command, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepo.GetByIdWithDetailsAsync(command.Id);
        if (doctor == null) throw new KeyNotFoundException($"Doctor with ID {command.Id} not found");

        var department = await _deptRepo.GetByIdAsync(command.DepartmentId);
        if (department == null) throw new KeyNotFoundException($"Department with ID {command.DepartmentId} not found");

        // Update User info
        doctor.User.FullName = command.FullName;
        doctor.User.Email = command.Email;
        doctor.User.UpdatedAt = DateTime.UtcNow;
        doctor.User.UpdatedBy = "Admin";

        // Update Doctor info
        doctor.Specialization = command.Specialization;
        doctor.DepartmentId = command.DepartmentId;
        doctor.AppointmentSlotDurationMinutes = command.SlotDurationMinutes;
        doctor.UpdatedAt = DateTime.UtcNow;
        doctor.UpdatedBy = "Admin";

        // Since we are updating aggregates, we persist changes.
        // EF Core tracking will handle User update via Doctor navigation if configured correctly, 
        // but explicit Update on User might be safer if generic repo attaches/detaches. 
        // Assuming tracking is on for retrieved entities.
        
         _doctorRepo.Update(doctor);
         _userRepo.Update(doctor.User); 
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public class UpdateDoctorCommandValidator : AbstractValidator<UpdateDoctorCommand>
{
    public UpdateDoctorCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.DepartmentId).GreaterThan(0);
        RuleFor(x => x.SlotDurationMinutes).InclusiveBetween(10, 60);
    }
}
