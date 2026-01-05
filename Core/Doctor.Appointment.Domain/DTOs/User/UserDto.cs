using System;
using System.Collections.Generic;

namespace Doctor.Appointment.Domain.DTOs.User
{
    public class UserDto
    {
        public long Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public long? PatientId { get; set; }
        public long? DoctorId { get; set; }
    }
}