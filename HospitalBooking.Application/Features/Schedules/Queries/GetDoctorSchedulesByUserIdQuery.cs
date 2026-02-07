namespace HospitalBooking.Application.Features.Schedules.Queries;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Schedules.DTOs;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;

public record GetDoctorSchedulesByUserIdQuery(int UserId) : IQuery<IEnumerable<DoctorScheduleDto>>;

public class GetDoctorSchedulesByUserIdQueryHandler : IQueryHandler<GetDoctorSchedulesByUserIdQuery, IEnumerable<DoctorScheduleDto>>
{
    private readonly IRepository<Doctor> _doctorRepo;
    private readonly IRepository<DoctorSchedule> _scheduleRepo;

    public GetDoctorSchedulesByUserIdQueryHandler(
        IRepository<Doctor> doctorRepo,
        IRepository<DoctorSchedule> scheduleRepo)
    {
        _doctorRepo = doctorRepo;
        _scheduleRepo = scheduleRepo;
    }

    public async Task<IEnumerable<DoctorScheduleDto>> Handle(GetDoctorSchedulesByUserIdQuery query, CancellationToken cancellationToken)
    {
        var doctors = await _doctorRepo.FindAsync(d => d.UserId == query.UserId);
        var doctor = doctors.FirstOrDefault();
        
        if (doctor == null) return Enumerable.Empty<DoctorScheduleDto>();

        var schedules = await _scheduleRepo.FindAsync(s => s.DoctorId == doctor.Id);
        
        return schedules.Select(s => new DoctorScheduleDto(
            s.Id, 
            s.DoctorId, 
            s.DayOfWeek, 
            s.StartTime.ToString(@"hh\:mm"), 
            s.EndTime.ToString(@"hh\:mm"), 
            s.IsAvailable));
    }
}
