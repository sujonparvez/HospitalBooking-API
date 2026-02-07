namespace HospitalBooking.Infrastructure.Persistence.Repositories;

using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
    {
        return await _dbSet
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Include(a => a.Patient)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ThenByDescending(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
    {
        return await _dbSet
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.AppointmentDate)
            .ThenByDescending(a => a.StartTime)
            .ToListAsync();
    }

     public async Task<IEnumerable<Appointment>> GetByDoctorIdAndDateAsync(int doctorId, DateTime date)
    {
         return await _dbSet
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId && a.AppointmentDate == date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetAllWithDetailsAsync(int? doctorId, DateTime? date)
    {
        var query = _dbSet
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Include(a => a.Patient)
            .AsQueryable();

        if (doctorId.HasValue)
        {
            query = query.Where(a => a.DoctorId == doctorId.Value);
        }

        if (date.HasValue)
        {
            query = query.Where(a => a.AppointmentDate == date.Value.Date);
        }

        return await query
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<bool> HasOverlapAsync(int doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime)
    {
        return await _dbSet.AnyAsync(a => 
            a.DoctorId == doctorId && 
            a.AppointmentDate == date && 
            a.Status != AppointmentStatus.Cancelled &&
            (a.StartTime < endTime && a.EndTime > startTime));
    }
}
