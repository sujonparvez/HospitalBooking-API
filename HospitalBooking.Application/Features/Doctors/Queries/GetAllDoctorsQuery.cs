namespace HospitalBooking.Application.Features.Doctors.Queries;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Doctors.DTOs;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;
// Ideally Repository should expose IQueryable or Include capabilities. 
// For MVP, if GenericRepository only returns IEnumerable/List, we might need a Specific Repository or enhance GenericRepository.
// Or we can rely on Lazy Loading if enabled (not enabled by default) or just fetch all and filter (bad perf but MVP).
// Let's assume we might need to modify Repository to support Includes or create DoctorRepository.

public record GetAllDoctorsQuery(int? DepartmentId) : IQuery<IEnumerable<DoctorDto>>;

public class GetAllDoctorsQueryHandler : IQueryHandler<GetAllDoctorsQuery, IEnumerable<DoctorDto>>
{
    private readonly IDoctorRepository _repository;

    public GetAllDoctorsQueryHandler(IDoctorRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DoctorDto>> Handle(GetAllDoctorsQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<Doctor> doctors;
        
        if (query.DepartmentId.HasValue)
        {
            doctors = await _repository.GetByDepartmentIdAsync(query.DepartmentId.Value);
        }
        else
        {
            doctors = await _repository.GetAllWithDetailsAsync();
        }

        return doctors.Select(d => new DoctorDto(
            d.Id, 
            d.User.FullName, 
            d.User.Email, 
            d.Specialization, 
            d.Department.Id, 
            d.Department.Name,
            d.AppointmentSlotDurationMinutes));
    }
}
