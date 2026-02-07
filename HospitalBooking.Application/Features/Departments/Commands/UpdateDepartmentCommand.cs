namespace HospitalBooking.Application.Features.Departments.Commands;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;
using FluentValidation;

public record UpdateDepartmentCommand(int Id, string Name, string Description, bool IsActive) : ICommand;

public class UpdateDepartmentCommandHandler : ICommandHandler<UpdateDepartmentCommand>
{
    private readonly IRepository<Department> _repository;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateDepartmentCommandHandler(IRepository<Department> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var department = await _repository.GetByIdAsync(command.Id);
        if (department == null)
        {
            throw new KeyNotFoundException($"Department with ID {command.Id} not found.");
        }

        department.Name = command.Name;
        department.Description = command.Description;
        department.IsActive = command.IsActive;
        department.UpdatedBy = "Admin"; // TODO: CurrentUser
        department.UpdatedAt = DateTime.UtcNow;

        _repository.Update(department);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
    }
}
