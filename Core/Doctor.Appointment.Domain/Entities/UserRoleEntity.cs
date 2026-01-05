using System;

namespace Doctor.Appointment.Domain.Entities
{
    public class UserRoleEntity
    {
        public long Id { get; set; }
        
        public long UserId { get; set; }
        public UserEntity? User { get; set; }
        
        public long RoleId { get; set; }
        public RoleEntity? Role { get; set; }
        
        public DateTime AssignedAt { get; set; }
    }
}