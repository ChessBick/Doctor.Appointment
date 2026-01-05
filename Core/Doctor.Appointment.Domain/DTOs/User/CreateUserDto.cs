using System.ComponentModel.DataAnnotations;

namespace Doctor.Appointment.Domain.DTOs.User
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(100)]
        public string? Username { get; set; }
        
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string? Password { get; set; }
        
        [Required]
        public List<long> RoleIds { get; set; } = new List<long>();
        
        public long? PatientId { get; set; }
        public long? DoctorId { get; set; }
    }
}