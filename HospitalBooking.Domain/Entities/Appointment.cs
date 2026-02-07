namespace HospitalBooking.Domain.Entities;

using HospitalBooking.Domain.Common;

public enum AppointmentStatus
{
    Pending = 1,
    Confirmed = 2,
    Cancelled = 3,
    Visited = 4,
    NoShow = 5
}

public class Appointment : BaseEntity
{
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public int PatientId { get; set; }
    public User Patient { get; set; } = null!;

    public DateTime AppointmentDate { get; set; }
    
    // Specific time slot for the appointment
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    
    public string? Reason { get; set; }
    public string? DoctorNotes { get; set; }
}
