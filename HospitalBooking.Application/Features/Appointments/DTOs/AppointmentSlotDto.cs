namespace HospitalBooking.Application.Features.Appointments.DTOs;

public record AppointmentSlotDto(
    string StartTime, 
    string EndTime, 
    bool IsAvailable);
