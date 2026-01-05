using Doctor.Appointment.Domain.DTOs.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doctor.Appointment.Application.Services.IServices
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(long id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto> UpdateUserAsync(UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(long id);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> LogoutAsync(long userId);
        Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string roleName);
        Task<bool> AssignRoleToUserAsync(long userId, long roleId);
        Task<bool> RemoveRoleFromUserAsync(long userId, long roleId);
        Task<bool> LockUserAsync(long userId);
        Task<bool> UnlockUserAsync(long userId);
    }
}