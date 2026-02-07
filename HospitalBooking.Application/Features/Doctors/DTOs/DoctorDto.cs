namespace HospitalBooking.Application.Features.Doctors.DTOs;

public record DoctorDto(
    int Id, 
    string FullName, 
    string Email, 
    string Specialization, 
    int DepartmentId, 
    string DepartmentName,
    int AppointmentSlotDurationMinutes);
