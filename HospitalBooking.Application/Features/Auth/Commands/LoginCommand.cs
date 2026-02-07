namespace HospitalBooking.Application.Features.Auth.Commands;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Auth.DTOs;
using HospitalBooking.Application.Interfaces.Auth;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;
using FluentValidation;

public record LoginCommand(string Email, string Password) : ICommand<AuthResponseDto>;

public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResponseDto>
{
    private readonly IRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    public LoginCommandHandler(
        IRepository<User> userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var users = await _userRepository.FindAsync(u => u.Email == command.Email);
        var user = users.FirstOrDefault();

        if (user == null || !_passwordHasher.VerifyPassword(command.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        return new AuthResponseDto(token, user.FullName, user.Role.ToString());
    }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
