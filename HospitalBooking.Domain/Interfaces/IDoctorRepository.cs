namespace HospitalBooking.Domain.Interfaces;

using HospitalBooking.Domain.Entities;

public interface IDoctorRepository:IRepository<Doctor>
{
    Task<IEnumerable<Doctor>> GetAllWithDetailsAsync();
    Task<Doctor?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Doctor>> GetByDepartmentIdAsync(int departmentId);
}