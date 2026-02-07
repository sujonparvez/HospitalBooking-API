namespace HospitalBooking.Application.Features.Schedules.Commands;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Schedules.DTOs;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;
using FluentValidation;

public record ManageDoctorScheduleCommand(
    int DoctorId,
    DayOfWeek DayOfWeek,
    string StartTime,
    string EndTime,
    bool IsAvailable) : ICommand<DoctorScheduleDto>;

public class ManageDoctorScheduleCommandHandler : ICommandHandler<ManageDoctorScheduleCommand, DoctorScheduleDto>
{
    private readonly IRepository<DoctorSchedule> _scheduleRepo;
    private readonly IRepository<Doctor> _doctorRepo;
    private readonly IUnitOfWork _unitOfWork;
    public ManageDoctorScheduleCommandHandler(IRepository<DoctorSchedule> scheduleRepo, IRepository<Doctor> doctorRepo, IUnitOfWork unitOfWork)
    {
        _scheduleRepo = scheduleRepo;
        _doctorRepo = doctorRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<DoctorScheduleDto> Handle(ManageDoctorScheduleCommand command, CancellationToken cancellationToken)
    {
        var doctor = _doctorRepo.Query().Where(m => m.UserId == command.DoctorId).FirstOrDefault();
        if (doctor == null)
        {
            throw new Exception("Invalid request. Doctor not found.");
        }
        // Try to find existing schedule for this day
        var schedules = await _scheduleRepo.FindAsync(x => x.DoctorId == doctor.Id && x.DayOfWeek == command.DayOfWeek);
        var schedule = schedules.FirstOrDefault();

        var startTime = TimeSpan.Parse(command.StartTime);
        var endTime = TimeSpan.Parse(command.EndTime);

        if (schedule == null)
        {
            schedule = new DoctorSchedule
            {
                DoctorId = doctor.Id,
                DayOfWeek = command.DayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                IsAvailable = command.IsAvailable,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Admin"
            };
            await _scheduleRepo.AddAsync(schedule);
        }
        else
        {
            schedule.StartTime = startTime;
            schedule.EndTime = endTime;
            schedule.IsAvailable = command.IsAvailable;
            schedule.UpdatedAt = DateTime.UtcNow;
            schedule.UpdatedBy = "Admin";
             _scheduleRepo.Update(schedule);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new DoctorScheduleDto(
            schedule.Id,
            schedule.DoctorId,
            schedule.DayOfWeek,
            schedule.StartTime.ToString(@"hh\:mm"),
            schedule.EndTime.ToString(@"hh\:mm"),
            schedule.IsAvailable);
    }
}

public class ManageDoctorScheduleCommandValidator : AbstractValidator<ManageDoctorScheduleCommand>
{
    public ManageDoctorScheduleCommandValidator()
    {
        RuleFor(x => x.DoctorId).GreaterThan(0);
        RuleFor(x => x.StartTime).Matches(@"^([01]\d|2[0-3]):([0-5]\d)$");
        RuleFor(x => x.EndTime).Matches(@"^([01]\d|2[0-3]):([0-5]\d)$");
        // EndTime > StartTime check could be added here or in handler logic
    }
}
