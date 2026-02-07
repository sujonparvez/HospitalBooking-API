namespace HospitalBooking.Application.Features.Appointments.Commands;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;

public record CancelAppointmentCommand(int AppointmentId, string Reason) : ICommand;

public class CancelAppointmentCommandHandler : ICommandHandler<CancelAppointmentCommand>
{
    private readonly IRepository<Appointment> _repository;
    private readonly IUnitOfWork _unitOfWork;
    public CancelAppointmentCommandHandler(IRepository<Appointment> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CancelAppointmentCommand command, CancellationToken cancellationToken)
    {
        var appt = await _repository.GetByIdAsync(command.AppointmentId);
        if (appt == null) throw new KeyNotFoundException("Appointment not found");

        if (appt.Status == AppointmentStatus.Visited || appt.Status == AppointmentStatus.Cancelled)
        {
             // Already done/cancelled
             // Idempotency or error? Let's just return or throwIfStrict.
             return; 
        }

        appt.Status = AppointmentStatus.Cancelled;
        appt.DoctorNotes = string.IsNullOrEmpty(appt.DoctorNotes) ? $"Cancelled: {command.Reason}" : $"{appt.DoctorNotes}; Cancelled: {command.Reason}";
        appt.UpdatedBy = "System"; // TODO: Should be CurrentUser
        appt.UpdatedAt = DateTime.UtcNow;

        _repository.Update(appt);
        await _unitOfWork.SaveChangesAsync();
    }
}
