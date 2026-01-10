using System;
using System.ComponentModel.DataAnnotations;

namespace Doctor.Appointment.Domain.DTOs.User
{
    public class UpdateUserDto
    {
        [Required]
        public long Id { get; set; }
        
        // Authentication properties
        [StringLength(100)]
        public string? Username { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        // Personal information properties
        [StringLength(100)]
        public string? FirstName { get; set; }
        
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
        
        // Doctor-specific properties
        [StringLength(50)]
        public string? PracticeNumber { get; set; }
        
        [StringLength(200)]
        public string? Qualification { get; set; }
        
        [StringLength(100)]
        public string? Specialization { get; set; }
        
        // Account status properties
        public bool? IsActive { get; set; }
        public bool? IsLocked { get; set; }
        
        // Role assignments
        public List<long>? RoleIds { get; set; }
    }
}