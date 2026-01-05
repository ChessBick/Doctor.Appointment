using System.ComponentModel.DataAnnotations;

namespace Doctor.Appointment.Domain.DTOs.User
{
    public class LoginDto
    {
        [Required]
        public string? UsernameOrEmail { get; set; }
        
        [Required]
        public string? Password { get; set; }
    }
}