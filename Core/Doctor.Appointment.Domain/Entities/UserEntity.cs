using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Doctor.Appointment.Domain.Entities
{
    public class UserEntity
    {
        public long Id { get; set; }
        
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
        
        // Navigation properties
        public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
        
        // Foreign key to link with Patient or Doctor
        public long? PatientId { get; set; }
        public PatientEntity? Patient { get; set; }
        
        public long? DoctorId { get; set; }
        public DoctorEntity? Doctor { get; set; }
    }
}