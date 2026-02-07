namespace HospitalBooking.API.Controllers;

using HospitalBooking.Application.Common.Models;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Appointments.Commands;
using HospitalBooking.Application.Features.Appointments.Queries;
using HospitalBooking.Application.Features.Doctors.Queries; // For searching doctors
using HospitalBooking.Application.Features.Departments.Queries; // For departments
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HospitalBooking.Application.Features.Departments.DTOs;
using HospitalBooking.Application.Features.Doctors.DTOs;
using HospitalBooking.Application.Features.Appointments.DTOs;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Patient")]
public class PatientController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public PatientController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    #region Search & Browse

    [HttpGet("departments")]
    [AllowAnonymous] 
    public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentDto>>>> GetDepartments()
    {
        var result = await _dispatcher.Query(new GetAllDepartmentsQuery());
        return Ok(new ApiResponse<IEnumerable<DepartmentDto>>(result));
    }
    
    [HttpGet("doctors")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<IEnumerable<DoctorDto>>>> GetDoctors([FromQuery] int? departmentId)
    {
        var result = await _dispatcher.Query(new GetAllDoctorsQuery(departmentId));
        return Ok(new ApiResponse<IEnumerable<DoctorDto>>(result));
    }

    [HttpGet("doctors/{doctorId}/slots")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AppointmentSlotDto>>>> GetSlots(int doctorId, [FromQuery] DateTime date)
    {
        var result = await _dispatcher.Query(new GetAvailableSlotsQuery(doctorId, date));
        return Ok(new ApiResponse<IEnumerable<AppointmentSlotDto>>(result));
    }

    #endregion

    #region Booking

    [HttpPost("appointments")]
    public async Task<ActionResult<ApiResponse<object>>> BookAppointment([FromBody] BookAppointmentCommand command)
    {
        // Enforce PatientId matches token
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (command.PatientId != userId) return Forbid();

        var appointmentId = await _dispatcher.Send(command);
        return Ok(new ApiResponse<object>(new { AppointmentId = appointmentId }, "Appointment Booked"));
    }

    [HttpGet("my-appointments")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AppointmentDto>>>> MyAppointments()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _dispatcher.Query(new GetPatientAppointmentsQuery(userId));
        return Ok(new ApiResponse<IEnumerable<AppointmentDto>>(result));
    }

    [HttpPost("appointments/{id}/cancel")]
    public async Task<ActionResult<ApiResponse>> CancelAppointment(int id, [FromBody] string reason)
    {
        // In a real app, verify ownership with Query first or inside Handler
        await _dispatcher.Send(new CancelAppointmentCommand(id, reason));
        return Ok(new ApiResponse("Appointment Cancelled"));
    }

    #endregion
}
