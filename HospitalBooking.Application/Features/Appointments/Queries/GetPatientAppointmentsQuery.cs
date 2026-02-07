namespace HospitalBooking.Application.Features.Appointments.Queries;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Appointments.DTOs;
using HospitalBooking.Domain.Interfaces;

public record GetPatientAppointmentsQuery(int PatientId) : IQuery<IEnumerable<AppointmentDto>>;

public class GetPatientAppointmentsQueryHandler : IQueryHandler<GetPatientAppointmentsQuery, IEnumerable<AppointmentDto>>
{
    private readonly IAppointmentRepository _repository;

    public GetPatientAppointmentsQueryHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AppointmentDto>> Handle(GetPatientAppointmentsQuery query, CancellationToken cancellationToken)
    {
        var appointments = await _repository.GetByPatientIdAsync(query.PatientId);

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
