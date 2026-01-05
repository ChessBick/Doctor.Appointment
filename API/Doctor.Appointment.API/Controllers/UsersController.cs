using Doctor.Appointment.Application.Services.IServices;
using Doctor.Appointment.Domain.DTOs.User;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doctor.Appointment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(long id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while retrieving the user" });
            }
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of all users</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while retrieving users" });
            }
        }

        /// <summary>
        /// Get users by role
        /// </summary>
        /// <param name="roleName">Role name (e.g., Patient, Doctor, Admin)</param>
        /// <returns>List of users with the specified role</returns>
        [HttpGet("role/{roleName}")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersByRole(string roleName)
        {
            try
            {
                var users = await _userService.GetUsersByRoleAsync(roleName);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users with role {RoleName}", roleName);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while retrieving users by role" });
            }
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="createUserDto">User creation details</param>
        /// <returns>Created user</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while creating user");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while creating the user" });
            }
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="updateUserDto">User update details</param>
        /// <returns>Updated user</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                if (id != updateUserDto.Id)
                {
                    return BadRequest(new { message = "ID mismatch" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userService.UpdateUserAsync(updateUserDto);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while updating user {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while updating the user" });
            }
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(long id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while deleting the user" });
            }
        }

        /// <summary>
        /// User login
        /// </summary>
        /// <param name="loginDto">Login credentials</param>
        /// <returns>Login response with token</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _userService.LoginAsync(loginDto);
                
                if (!response.Success)
                {
                    return Unauthorized(new { message = response.Message });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login attempt");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred during login" });
            }
        }

        /// <summary>
        /// User logout
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Success status</returns>
        [HttpPost("logout/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Logout(long userId)
        {
            try
            {
                var result = await _userService.LogoutAsync(userId);
                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred during logout" });
            }
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="changePasswordDto">Password change details</param>
        /// <returns>Success status</returns>
        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userService.ChangePasswordAsync(changePasswordDto);
                return Ok(new { message = "Password changed successfully" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while changing password");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while changing password" });
            }
        }

        /// <summary>
        /// Assign role to user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="roleId">Role ID</param>
        /// <returns>Success status</returns>
        [HttpPost("{userId}/roles/{roleId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignRoleToUser(long userId, long roleId)
        {
            try
            {
                var result = await _userService.AssignRoleToUserAsync(userId, roleId);
                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "Role assigned successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while assigning role" });
            }
        }

        /// <summary>
        /// Remove role from user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="roleId">Role ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{userId}/roles/{roleId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveRoleFromUser(long userId, long roleId)
        {
            try
            {
                var result = await _userService.RemoveRoleFromUserAsync(userId, roleId);
                if (!result)
                {
                    return NotFound(new { message = "User or role not found" });
                }

                return Ok(new { message = "Role removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while removing role" });
            }
        }

        /// <summary>
        /// Lock user account
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Success status</returns>
        [HttpPost("{userId}/lock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LockUser(long userId)
        {
            try
            {
                var result = await _userService.LockUserAsync(userId);
                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "User locked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error locking user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while locking user" });
            }
        }

        /// <summary>
        /// Unlock user account
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Success status</returns>
        [HttpPost("{userId}/unlock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnlockUser(long userId)
        {
            try
            {
                var result = await _userService.UnlockUserAsync(userId);
                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "User unlocked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlocking user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while unlocking user" });
            }
        }
    }
}