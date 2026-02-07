namespace HospitalBooking.Domain.Interfaces;

using HospitalBooking.Domain.Entities;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
    Task<IEnumerable<Appointment>> GetByDoctorIdAndDateAsync(int doctorId, DateTime date);
    Task<IEnumerable<Appointment>> GetAllWithDetailsAsync(int? doctorId, DateTime? date);
    Task<bool> HasOverlapAsync(int doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime);
}
