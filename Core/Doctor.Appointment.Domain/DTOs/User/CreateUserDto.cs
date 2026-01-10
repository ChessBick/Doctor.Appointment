using System;
using System.ComponentModel.DataAnnotations;

namespace Doctor.Appointment.Domain.DTOs.User
{
    public class CreateUserDto
    {
        // Authentication properties
        [Required]
        [StringLength(100)]
        public string? Username { get; set; }
        
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string? Password { get; set; }
        
        // Personal information properties
        [Required]
        [StringLength(100)]
        public string? FirstName { get; set; }
        
        [Required]
        [StringLength(100)]
        public string? LastName { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        [StringLength(20)]
        public string? IdNumber { get; set; }
        
        [StringLength(200)]
        public string? Address { get; set; }
        
        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        
        // Doctor-specific properties (only applicable for users with Doctor role)
        [StringLength(50)]
        public string? PracticeNumber { get; set; }
        
        [StringLength(200)]
        public string? Qualification { get; set; }
        
        [StringLength(100)]
        public string? Specialization { get; set; }
        
        // Role assignment
        [Required]
        public List<long> RoleIds { get; set; } = new List<long>();
    }
}