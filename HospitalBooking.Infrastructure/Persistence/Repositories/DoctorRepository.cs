namespace HospitalBooking.Infrastructure.Persistence.Repositories;

using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;


public class DoctorRepository : Repository<Doctor>, IDoctorRepository
{
    public DoctorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Doctor>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(d => d.User)
            .Include(d => d.Department)
            .ToListAsync();
    }

    public async Task<Doctor?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(d => d.User)
            .Include(d => d.Department)
            .Include(d => d.Schedules) // Also useful for schedule management
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Doctor>> GetByDepartmentIdAsync(int departmentId)
    {
        return await _dbSet
            .Include(d => d.User)
            .Include(d => d.Department)
            .Where(d => d.DepartmentId == departmentId)
            .ToListAsync();
    }
}
