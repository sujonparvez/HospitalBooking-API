namespace HospitalBooking.Application.Features.Schedules.Queries;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Schedules.DTOs;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;

public record GetDoctorSchedulesQuery(int DoctorId) : IQuery<IEnumerable<DoctorScheduleDto>>;

public class GetDoctorSchedulesQueryHandler : IQueryHandler<GetDoctorSchedulesQuery, IEnumerable<DoctorScheduleDto>>
{
    private readonly IRepository<DoctorSchedule> _repository;

    public GetDoctorSchedulesQueryHandler(IRepository<DoctorSchedule> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DoctorScheduleDto>> Handle(GetDoctorSchedulesQuery query, CancellationToken cancellationToken)
    {
        var schedules = await _repository.FindAsync(s => s.DoctorId == query.DoctorId);
        
        return schedules.Select(s => new DoctorScheduleDto(
            s.Id, 
            s.DoctorId, 
            s.DayOfWeek, 
            s.StartTime.ToString(@"hh\:mm"), 
            s.EndTime.ToString(@"hh\:mm"), 
            s.IsAvailable));
    }
}
