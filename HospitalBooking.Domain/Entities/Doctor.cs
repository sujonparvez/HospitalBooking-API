namespace HospitalBooking.Domain.Entities;

using HospitalBooking.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

public class Doctor : BaseEntity
{
    // Foreign Key to User
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public string Specialization { get; set; } = string.Empty;
    public int AppointmentSlotDurationMinutes { get; set; } = 30; // Configurable per doctor

    // Navigation properties
    public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
