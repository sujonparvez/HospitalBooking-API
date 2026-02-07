namespace HospitalBooking.Domain.Entities;

using HospitalBooking.Domain.Common;

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
