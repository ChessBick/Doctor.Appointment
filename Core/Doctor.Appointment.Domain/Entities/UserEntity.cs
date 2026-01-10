using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Doctor.Appointment.Domain.Entities
{
    public class UserEntity
    {
        public long Id { get; set; }
        
        // Authentication properties
        [Required]
        [StringLength(100)]
        public string? Username { get; set; }
        
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        
        [Required]
        public string? PasswordHash { get; set; }
        
        public string? PasswordSalt { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? LastLoginAt { get; set; }
        
        public DateTime? PasswordChangedAt { get; set; }
        
        public bool IsActive { get; set; }
        
        public bool IsLocked { get; set; }
        
        public int FailedLoginAttempts { get; set; }
        
        // Personal information properties (from Patient)
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
        
        // Doctor-specific properties (only applicable if user has Doctor role)
        [StringLength(50)]
        public string? PracticeNumber { get; set; }
        
        [StringLength(200)]
        public string? Qualification { get; set; }
        
        [StringLength(100)]
        public string? Specialization { get; set; }
        
        // Navigation properties
        public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    }
}