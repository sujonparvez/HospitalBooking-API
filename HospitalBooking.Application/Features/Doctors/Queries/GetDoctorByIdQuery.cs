namespace HospitalBooking.Application.Features.Doctors.Queries;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Doctors.DTOs;
using HospitalBooking.Domain.Interfaces;

public record GetDoctorByIdQuery(int Id) : IQuery<DoctorDto>;

public class GetDoctorByIdQueryHandler : IQueryHandler<GetDoctorByIdQuery, DoctorDto>
{
    private readonly IDoctorRepository _repository;

    public GetDoctorByIdQueryHandler(IDoctorRepository repository)
    {
        _repository = repository;
    }

    public async Task<DoctorDto> Handle(GetDoctorByIdQuery query, CancellationToken cancellationToken)
    {
        var d = await _repository.GetByIdWithDetailsAsync(query.Id);
        if (d == null)
        {
            throw new KeyNotFoundException($"Doctor with ID {query.Id} not found.");
        }

        return new DoctorDto(
            d.Id, 
            d.User.FullName, 
            d.User.Email, 
            d.Specialization, 
            d.Department.Id, 
            d.Department.Name,
            d.AppointmentSlotDurationMinutes);
    }
}
