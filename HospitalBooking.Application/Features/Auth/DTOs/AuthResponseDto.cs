namespace HospitalBooking.Application.Features.Auth.DTOs;

public record AuthResponseDto(string Token, string FullName, string Role);
