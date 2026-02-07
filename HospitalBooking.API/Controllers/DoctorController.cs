using HospitalBooking.Application.Features.Schedules.DTOs;
using HospitalBooking.Application.Common.Models;

namespace HospitalBooking.API.Controllers;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Appointments.DTOs;
using HospitalBooking.Application.Features.Appointments.Queries;
using HospitalBooking.Application.Features.Schedules.Commands;
using HospitalBooking.Application.Features.Schedules.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Doctor")]
public class DoctorController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public DoctorController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet("my-schedule")]
    public async Task<ActionResult<ApiResponse<IEnumerable<DoctorScheduleDto>>>> GetMySchedule()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _dispatcher.Query(new GetDoctorSchedulesByUserIdQuery(userId));
        return Ok(new ApiResponse<IEnumerable<DoctorScheduleDto>>(result));
    }

    [HttpPost("schedule")]
    public async Task<ActionResult<ApiResponse<DoctorScheduleDto>>> UpdateMySchedule([FromBody] UpdateMyScheduleRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var command = new ManageDoctorScheduleCommand(
            userId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.IsAvailable);

        var result = await _dispatcher.Send(command);
        return Ok(new ApiResponse<DoctorScheduleDto>(result, "Schedule Updated"));
    }

    [HttpGet("my-appointments")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AppointmentDto>>>> MyAppointments()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _dispatcher.Query(new GetDoctorAppointmentsQuery(userId));
        return Ok(new ApiResponse<IEnumerable<AppointmentDto>>(result));
    }
}
