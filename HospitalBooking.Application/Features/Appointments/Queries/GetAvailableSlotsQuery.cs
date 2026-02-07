namespace HospitalBooking.Application.Features.Appointments.Queries;

using HospitalBooking.Application.Abstractions.CQRS;
using HospitalBooking.Application.Features.Appointments.DTOs;
using HospitalBooking.Domain.Entities;
using HospitalBooking.Domain.Interfaces;

public record GetAvailableSlotsQuery(int DoctorId, DateTime Date) : IQuery<IEnumerable<AppointmentSlotDto>>;

public class GetAvailableSlotsQueryHandler : IQueryHandler<GetAvailableSlotsQuery, IEnumerable<AppointmentSlotDto>>
{
    private readonly IRepository<Doctor> _doctorRepo;
    private readonly IRepository<DoctorSchedule> _scheduleRepo;
    private readonly IRepository<Appointment> _appointmentRepo;

    public GetAvailableSlotsQueryHandler(
        IRepository<Doctor> doctorRepo,
        IRepository<DoctorSchedule> scheduleRepo,
        IRepository<Appointment> appointmentRepo)
    {
        _doctorRepo = doctorRepo;
        _scheduleRepo = scheduleRepo;
        _appointmentRepo = appointmentRepo;
    }

    public async Task<IEnumerable<AppointmentSlotDto>> Handle(GetAvailableSlotsQuery query, CancellationToken cancellationToken)
    {
        var date = query.Date.Date;
        var dayOfWeek = date.DayOfWeek;

        // 1. Get Doctor Schedule
        var schedules = await _scheduleRepo.FindAsync(s => s.DoctorId == query.DoctorId && s.DayOfWeek == dayOfWeek && s.IsAvailable);
        var schedule = schedules.FirstOrDefault();

        if (schedule == null) return Enumerable.Empty<AppointmentSlotDto>();

        // 2. Get Doctor details (for slot duration)
        var doctor = await _doctorRepo.GetByIdAsync(query.DoctorId);
        if (doctor == null) return Enumerable.Empty<AppointmentSlotDto>();

        // 3. Get Existing Appointments
        var appointments = await _appointmentRepo.FindAsync(a => 
            a.DoctorId == query.DoctorId && 
            a.AppointmentDate == date && 
            a.Status != AppointmentStatus.Cancelled);

        // 4. Generate Slots
        var slots = new List<AppointmentSlotDto>();
        var currentStart = schedule.StartTime;
        var duration = TimeSpan.FromMinutes(doctor.AppointmentSlotDurationMinutes);

        while (currentStart + duration <= schedule.EndTime)
        {
            var end = currentStart + duration;
            
            // Check overlap
            bool isBooked = appointments.Any(a => 
                (a.StartTime < end && a.EndTime > currentStart)); // Standard overlap check

            if (!isBooked)
            {
                slots.Add(new AppointmentSlotDto(
                    currentStart.ToString(@"hh\:mm"), 
                    end.ToString(@"hh\:mm"), 
                    true));
            }

            currentStart = currentStart.Add(duration);
        }

        return slots;
    }
}
