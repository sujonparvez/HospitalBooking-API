namespace HospitalBooking.Application.Features.Departments.Commands;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Departments.DTOs;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;
using FluentValidation;

public record CreateDepartmentCommand(string Name, string Description) : ICommand<DepartmentDto>;

public class CreateDepartmentCommandHandler : ICommandHandler<CreateDepartmentCommand, DepartmentDto>
{
    private readonly IRepository<Department> _repository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateDepartmentCommandHandler(IRepository<Department> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DepartmentDto> Handle(CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var department = new Department
        {
            Name = command.Name,
            Description = command.Description,
            IsActive = true,
            CreatedBy = "Admin", // TODO: Get from CurrentUserService
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(department);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new DepartmentDto(department.Id, department.Name, department.Description, department.IsActive);
    }
}

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
    }
}
