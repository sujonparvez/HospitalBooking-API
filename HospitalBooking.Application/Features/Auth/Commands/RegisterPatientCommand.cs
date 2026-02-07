namespace HospitalBooking.Application.Features.Auth.Commands;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Auth.DTOs;
using HospitalBooking.Application.Interfaces.Auth;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;
using FluentValidation;

public record RegisterPatientCommand(
    string FullName, 
    string Email, 
    string Password, 
    string PhoneNumber) : ICommand<AuthResponseDto>;

public class RegisterPatientCommandHandler : ICommandHandler<RegisterPatientCommand, AuthResponseDto>
{
    private readonly IRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    public RegisterPatientCommandHandler(
        IRepository<User> userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponseDto> Handle(RegisterPatientCommand command, CancellationToken cancellationToken)
    {
        var existingUsers = await _userRepository.FindAsync(u => u.Email == command.Email);
        if (existingUsers.Any())
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var user = new User
        {
            FullName = command.FullName,
            Email = command.Email,
            PasswordHash = _passwordHasher.HashPassword(command.Password),
            PhoneNumber = command.PhoneNumber,
            Role = UserRole.Patient,
            CreatedBy = "System", // Self-registration
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        await _unitOfWork.SaveChangesAsync();

        var token = _jwtTokenGenerator.GenerateToken(user);
        return new AuthResponseDto(token, user.FullName, user.Role.ToString());
    }
}

public class RegisterPatientCommandValidator : AbstractValidator<RegisterPatientCommand>
{
    public RegisterPatientCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
    }
}
