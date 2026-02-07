namespace HospitalBooking.Application.Features.Departments.Queries;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Departments.DTOs;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;

public record GetDepartmentByIdQuery(int Id) : IQuery<DepartmentDto>;

public class GetDepartmentByIdQueryHandler : IQueryHandler<GetDepartmentByIdQuery, DepartmentDto>
{
    private readonly IRepository<Department> _repository;

    public GetDepartmentByIdQueryHandler(IRepository<Department> repository)
    {
        _repository = repository;
    }

    public async Task<DepartmentDto> Handle(GetDepartmentByIdQuery query, CancellationToken cancellationToken)
    {
        var d = await _repository.GetByIdAsync(query.Id);
        if (d == null)
        {
            throw new KeyNotFoundException($"Department with ID {query.Id} not found.");
        }
        return new DepartmentDto(d.Id, d.Name, d.Description, d.IsActive);
    }
}
