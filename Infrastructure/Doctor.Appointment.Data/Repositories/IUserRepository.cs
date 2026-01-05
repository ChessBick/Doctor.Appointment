using Doctor.Appointment.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doctor.Appointment.Data.Repositories
{
    public interface IUserRepository : IGenericRepository<UserEntity>
    {
        Task<UserEntity?> GetByUsernameAsync(string username);
        Task<UserEntity?> GetByEmailAsync(string email);
        Task<UserEntity?> GetByIdWithRolesAsync(long id);
        Task<UserEntity?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task<IEnumerable<UserEntity>> GetUsersByRoleAsync(long roleId);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }
}