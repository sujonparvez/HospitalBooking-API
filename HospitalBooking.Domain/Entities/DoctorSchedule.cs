namespace HospitalBooking.Domain.Entities;

using HospitalBooking.Domain.Common;

public class DoctorSchedule : BaseEntity
{
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }
    
    // Storing time as TimeSpan since it's time-of-day
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public bool IsAvailable { get; set; } = true;
}
