namespace HospitalBooking.Application.Features.Schedules.DTOs;

public record UpdateMyScheduleRequest(
    DayOfWeek DayOfWeek, 
    string StartTime, 
    string EndTime, 
    bool IsAvailable);
