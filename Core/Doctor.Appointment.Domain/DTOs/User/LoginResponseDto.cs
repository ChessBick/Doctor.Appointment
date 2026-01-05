using System;

namespace Doctor.Appointment.Domain.DTOs.User
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public UserDto? User { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}