namespace HospitalBooking.Application.Features.Appointments.Queries;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Appointments.DTOs;
using HospitalBooking.Domain.Interfaces;

public record GetAllAppointmentsQuery(int? DoctorId, DateTime? Date) : IQuery<IEnumerable<AppointmentDto>>;

public class GetAllAppointmentsQueryHandler : IQueryHandler<GetAllAppointmentsQuery, IEnumerable<AppointmentDto>>
{
    private readonly IAppointmentRepository _repository;

    public GetAllAppointmentsQueryHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AppointmentDto>> Handle(GetAllAppointmentsQuery query, CancellationToken cancellationToken)
    {
        var appointments = await _repository.GetAllWithDetailsAsync(query.DoctorId, query.Date);

        return appointments.Select(a => new AppointmentDto(
            a.Id,
            a.DoctorId,
            a.Doctor.User.FullName,
            a.Doctor.Specialization,
            a.PatientId,
            a.Patient.FullName,
            a.AppointmentDate,
            a.StartTime.ToString(@"hh\:mm"),
            a.EndTime.ToString(@"hh\:mm"),
            a.Status.ToString(),
            a.Reason,
            a.DoctorNotes));
    }
}
