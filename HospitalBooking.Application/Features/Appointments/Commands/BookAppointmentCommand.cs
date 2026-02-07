namespace HospitalBooking.Application.Features.Appointments.Commands;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;
using FluentValidation;

public record BookAppointmentCommand(
    int DoctorId,
    int PatientId,
    DateTime Date,
    string StartTime) : ICommand<int>;

public class BookAppointmentCommandHandler : ICommandHandler<BookAppointmentCommand, int>
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IRepository<Doctor> _doctorRepo;
    private readonly IUnitOfWork _unitOfWork;

    public BookAppointmentCommandHandler(
        IAppointmentRepository appointmentRepo,
        IRepository<Doctor> doctorRepo,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepo = appointmentRepo;
        _doctorRepo = doctorRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(BookAppointmentCommand command, CancellationToken cancellationToken)
    {
        // Start Transaction via Abstraction
        using var transaction = await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken);

        try
        {
            var date = command.Date.Date;
            var startTime = TimeSpan.Parse(command.StartTime);

            // 1. Get Doctor for duration
            var doctor = await _doctorRepo.GetByIdAsync(command.DoctorId);
            if (doctor == null) throw new KeyNotFoundException("Doctor not found");

            var endTime = startTime.Add(TimeSpan.FromMinutes(doctor.AppointmentSlotDurationMinutes));

            // 2. Check overlap
            // Using Repository method which should use the same context implicitly (dependency injection scoping)
            // However, Transaction object needs to be respected by Context. EF Core handles this if using same DbContext instance.
            bool overlap = await _appointmentRepo.HasOverlapAsync(command.DoctorId, date, startTime, endTime);

            if (overlap)
            {
                throw new InvalidOperationException("Slot is already booked.");
            }

            // 3. Create Appointment
            var appointment = new Appointment
            {
                DoctorId = command.DoctorId,
                PatientId = command.PatientId,
                AppointmentDate = date,
                StartTime = startTime,
                EndTime = endTime,
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            await _appointmentRepo.AddAsync(appointment);

            await _unitOfWork.SaveChangesAsync();

            await transaction.CommitAsync(cancellationToken);

            return appointment.Id;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
