namespace HospitalBooking.Application.Features.Appointments.Queries;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Appointments.DTOs;
using HospitalBooking.Domain.Interfaces;
using HospitalBooking.Domain.Entities; // Added

public record GetDoctorAppointmentsQuery(int DoctorUserId) : IQuery<IEnumerable<AppointmentDto>>;

public class GetDoctorAppointmentsQueryHandler : IQueryHandler<GetDoctorAppointmentsQuery, IEnumerable<AppointmentDto>>
{
    private readonly IRepository<Doctor> _doctorRepo;
    private readonly IAppointmentRepository _appointmentRepo;

    public GetDoctorAppointmentsQueryHandler(IRepository<Doctor> doctorRepo, IAppointmentRepository appointmentRepo)
    {
        _doctorRepo = doctorRepo;
        _appointmentRepo = appointmentRepo;
    }

    public async Task<IEnumerable<AppointmentDto>> Handle(GetDoctorAppointmentsQuery query, CancellationToken cancellationToken)
    {
        var doctors = await _doctorRepo.FindAsync(d => d.UserId == query.DoctorUserId);
        var doctor = doctors.FirstOrDefault();
        
        if (doctor == null) return Enumerable.Empty<AppointmentDto>();

        var appointments = await _appointmentRepo.GetByDoctorIdAsync(doctor.Id);

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
