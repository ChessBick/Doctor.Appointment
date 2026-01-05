using System.ComponentModel.DataAnnotations;

namespace Doctor.Appointment.Domain.DTOs.User
{
    public class ChangePasswordDto
    {
        [Required]
        public long UserId { get; set; }
        
        [Required]
        public string? CurrentPassword { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string? NewPassword { get; set; }
        
        [Required]
        [Compare("NewPassword")]
        public string? ConfirmPassword { get; set; }
    }
}