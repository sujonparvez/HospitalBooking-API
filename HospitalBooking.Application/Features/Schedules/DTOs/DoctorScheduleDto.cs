namespace HospitalBooking.Application.Features.Schedules.DTOs;

public record DoctorScheduleDto(
    int Id, 
    int DoctorId, 
    DayOfWeek DayOfWeek, 
    string StartTime, // "HH:mm"
    string EndTime,   // "HH:mm"
    bool IsAvailable);
