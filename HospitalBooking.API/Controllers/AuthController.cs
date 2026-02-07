namespace HospitalBooking.API.Controllers;

using HospitalBooking.Application.Common.Models; // Added

using HospitalBooking.Application.Features.Auth.DTOs;
using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Auth.Commands;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public AuthController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }



    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginCommand command)
    {
        var result = await _dispatcher.Send(command);
        return Ok(new ApiResponse<AuthResponseDto>(result, "Login Successful"));
    }

    [HttpPost("register-patient")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterPatientCommand command)
    {
        var result = await _dispatcher.Send(command);
        return Ok(new ApiResponse<AuthResponseDto>(result, "Registration Successful"));
    }
}
