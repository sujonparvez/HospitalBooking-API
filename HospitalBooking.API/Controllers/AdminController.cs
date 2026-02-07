namespace HospitalBooking.API.Controllers;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Departments.Commands;
using HospitalBooking.Application.Features.Departments.Queries;
using HospitalBooking.Application.Features.Doctors.Commands; // Added
using HospitalBooking.Application.Features.Doctors.Queries;  // Added
using HospitalBooking.Application.Features.Schedules.Commands; // Added
using HospitalBooking.Application.Features.Schedules.Queries;  // Added
using HospitalBooking.Application.Features.Appointments.Queries; // Added
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public AdminController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    #region Departments

    [HttpGet("departments")]
    public async Task<IActionResult> GetAllDepartments()
    {
        var result = await _dispatcher.Query(new GetAllDepartmentsQuery());
        return Ok(result);
    }

    [HttpGet("departments/{id}")]
    public async Task<IActionResult> GetDepartmentById(int id)
    {
        var result = await _dispatcher.Query(new GetDepartmentByIdQuery(id));
        return Ok(result);
    }

    [HttpPost("departments")]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentCommand command)
    {
        var result = await _dispatcher.Send(command);
        return CreatedAtAction(nameof(GetDepartmentById), new { id = result.Id }, result);
    }

    [HttpPut("departments/{id}")]
    public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UpdateDepartmentCommand command)
    {
        if (id != command.Id) return BadRequest("ID Mismatch");
        await _dispatcher.Send(command);
        return NoContent();
    }

    [HttpDelete("departments/{id}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        await _dispatcher.Send(new DeleteDepartmentCommand(id));
        return NoContent();
    }

    #endregion

    #region Doctors

    [HttpGet("doctors")]
    public async Task<IActionResult> GetAllDoctors([FromQuery] int? departmentId)
    {
        var result = await _dispatcher.Query(new GetAllDoctorsQuery(departmentId));
        return Ok(result);
    }

    [HttpGet("doctors/{id}")]
    public async Task<IActionResult> GetDoctorById(int id)
    {
        var result = await _dispatcher.Query(new GetDoctorByIdQuery(id));
        return Ok(result);
    }

    [HttpPost("doctors")]
    public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorCommand command)
    {
        var result = await _dispatcher.Send(command);
        return CreatedAtAction(nameof(GetDoctorById), new { id = result.Id }, result);
    }

    [HttpPut("doctors/{id}")]
    public async Task<IActionResult> UpdateDoctor(int id, [FromBody] UpdateDoctorCommand command)
    {
        if (id != command.Id) return BadRequest("ID Mismatch");
        await _dispatcher.Send(command);
        return NoContent();
    }

    #endregion

    #region Schedules

    [HttpGet("doctors/{doctorId}/schedules")]
    public async Task<IActionResult> GetDoctorSchedules(int doctorId)
    {
        var result = await _dispatcher.Query(new GetDoctorSchedulesQuery(doctorId));
        return Ok(result);
    }

    [HttpPost("schedules")]
    public async Task<IActionResult> ManageSchedule([FromBody] ManageDoctorScheduleCommand command)
    {
        var result = await _dispatcher.Send(command);
        return Ok(result);
    }

    #endregion

    #region Appointments

    [HttpGet("appointments")]
    public async Task<IActionResult> GetAllAppointments([FromQuery] int? doctorId, [FromQuery] DateTime? date)
    {
        var result = await _dispatcher.Query(new GetAllAppointmentsQuery(doctorId, date));
        return Ok(result);
    }

    #endregion
}
