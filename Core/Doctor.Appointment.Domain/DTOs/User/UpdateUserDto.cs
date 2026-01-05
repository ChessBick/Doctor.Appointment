using System.ComponentModel.DataAnnotations;

namespace Doctor.Appointment.Domain.DTOs.User
{
    public class UpdateUserDto
    {
        [Required]
        public long Id { get; set; }
        
        [StringLength(100)]
        public string? Username { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        public bool? IsActive { get; set; }
        public bool? IsLocked { get; set; }
        
        public List<long>? RoleIds { get; set; }
    }
}