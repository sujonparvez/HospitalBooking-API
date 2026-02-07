namespace HospitalBooking.Application.Features.Departments.Queries;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Departments.DTOs;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;

public record GetAllDepartmentsQuery : IQuery<IEnumerable<DepartmentDto>>;

public class GetAllDepartmentsQueryHandler : IQueryHandler<GetAllDepartmentsQuery, IEnumerable<DepartmentDto>>
{
    private readonly IRepository<Department> _repository;

    public GetAllDepartmentsQueryHandler(IRepository<Department> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DepartmentDto>> Handle(GetAllDepartmentsQuery query, CancellationToken cancellationToken)
    {
        var departments = await _repository.GetAllAsync();
        return departments.Select(d => new DepartmentDto(d.Id, d.Name, d.Description, d.IsActive));
    }
}
