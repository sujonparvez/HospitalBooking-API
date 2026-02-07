namespace HospitalBooking.Application.Features.Appointments.DTOs;

public record AppointmentDto(
    int Id, 
    int DoctorId, 
    string DoctorName, 
    string DoctorSpecialization,
    int PatientId, 
    string PatientName,
    DateTime Date, 
    string StartTime, 
    string EndTime, 
    string Status,
    string? Reason,
    string? DoctorNotes);
