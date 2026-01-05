using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Doctor.Appointment.Domain.Entities
{
    public class RoleEntity
    {
        public long Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string? RoleName { get; set; }
        
        public string? Description { get; set; }
        
        // Navigation property
        public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    }
}