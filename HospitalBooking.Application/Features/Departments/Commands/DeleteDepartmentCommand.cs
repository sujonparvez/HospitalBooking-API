namespace HospitalBooking.Application.Features.Departments.Commands;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;

public record DeleteDepartmentCommand(int Id) : ICommand;

public class DeleteDepartmentCommandHandler : ICommandHandler<DeleteDepartmentCommand>
{
    private readonly IRepository<Department> _repository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteDepartmentCommandHandler(IRepository<Department> repository,IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteDepartmentCommand command, CancellationToken cancellationToken)
    {
        var department = await _repository.GetByIdAsync(command.Id);
        if (department == null)
        {
            throw new KeyNotFoundException($"Department with ID {command.Id} not found.");
        }

        // Soft delete
        department.IsDeleted = true;
        _repository.Update(department);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
