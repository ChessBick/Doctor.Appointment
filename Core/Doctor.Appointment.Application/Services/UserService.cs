using AutoMapper;
using Doctor.Appointment.Application.Services.IServices;
using Doctor.Appointment.Data.Repositories;
using Doctor.Appointment.Domain.DTOs.User;
using Doctor.Appointment.Domain.Entities;

namespace Doctor.Appointment.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IUserRepository userRepository, 
            IMapper mapper,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto?> GetUserByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(id);
            if (user == null) return null;

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = user.UserRoles.Select(ur => ur.Role?.RoleName ?? string.Empty).ToList();
            return userDto;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = _userRepository.GetAll();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var userWithRoles = await _userRepository.GetByIdWithRolesAsync(user.Id);
                if (userWithRoles != null)
                {
                    var userDto = _mapper.Map<UserDto>(userWithRoles);
                    userDto.Roles = userWithRoles.UserRoles.Select(ur => ur.Role?.RoleName ?? string.Empty).ToList();
                    userDtos.Add(userDto);
                }
            }

            return userDtos;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Validate username and email uniqueness
            if (await _userRepository.UsernameExistsAsync(createUserDto.Username ?? string.Empty))
                throw new InvalidOperationException("Username already exists");

            if (await _userRepository.EmailExistsAsync(createUserDto.Email ?? string.Empty))
                throw new InvalidOperationException("Email already exists");

            // Hash password
            var (passwordHash, passwordSalt) = _passwordHasher.HashPassword(createUserDto.Password ?? string.Empty);

            var userEntity = new UserEntity
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsLocked = false,
                FailedLoginAttempts = 0,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                DateOfBirth = createUserDto.DateOfBirth,
                IdNumber = createUserDto.IdNumber,
                Address = createUserDto.Address,
                PhoneNumber = createUserDto.PhoneNumber,
                PracticeNumber = createUserDto.PracticeNumber,
                Qualification = createUserDto.Qualification,
                Specialization = createUserDto.Specialization
            };

            _userRepository.Insert(userEntity);
            await _userRepository.SaveAsync();

            // Assign roles
            foreach (var roleId in createUserDto.RoleIds)
            {
                await AssignRoleToUserAsync(userEntity.Id, roleId);
            }

            return await GetUserByIdAsync(userEntity.Id) ?? new UserDto();
        }

        public async Task<UserDto> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(updateUserDto.Id);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Update authentication fields
            if (!string.IsNullOrEmpty(updateUserDto.Username))
            {
                if (user.Username != updateUserDto.Username && 
                    await _userRepository.UsernameExistsAsync(updateUserDto.Username))
                    throw new InvalidOperationException("Username already exists");
                
                user.Username = updateUserDto.Username;
            }

            if (!string.IsNullOrEmpty(updateUserDto.Email))
            {
                if (user.Email != updateUserDto.Email && 
                    await _userRepository.EmailExistsAsync(updateUserDto.Email))
                    throw new InvalidOperationException("Email already exists");
                
                user.Email = updateUserDto.Email;
            }

            // Update personal information
            if (!string.IsNullOrEmpty(updateUserDto.FirstName))
                user.FirstName = updateUserDto.FirstName;

            if (!string.IsNullOrEmpty(updateUserDto.LastName))
                user.LastName = updateUserDto.LastName;

            if (updateUserDto.DateOfBirth.HasValue)
                user.DateOfBirth = updateUserDto.DateOfBirth;

            if (!string.IsNullOrEmpty(updateUserDto.IdNumber))
                user.IdNumber = updateUserDto.IdNumber;

            if (!string.IsNullOrEmpty(updateUserDto.Address))
                user.Address = updateUserDto.Address;

            if (!string.IsNullOrEmpty(updateUserDto.PhoneNumber))
                user.PhoneNumber = updateUserDto.PhoneNumber;

            // Update doctor-specific fields
            if (!string.IsNullOrEmpty(updateUserDto.PracticeNumber))
                user.PracticeNumber = updateUserDto.PracticeNumber;

            if (!string.IsNullOrEmpty(updateUserDto.Qualification))
                user.Qualification = updateUserDto.Qualification;

            if (!string.IsNullOrEmpty(updateUserDto.Specialization))
                user.Specialization = updateUserDto.Specialization;

            // Update account status
            if (updateUserDto.IsActive.HasValue)
                user.IsActive = updateUserDto.IsActive.Value;

            if (updateUserDto.IsLocked.HasValue)
                user.IsLocked = updateUserDto.IsLocked.Value;

            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            // Update roles if provided
            if (updateUserDto.RoleIds != null && updateUserDto.RoleIds.Any())
            {
                // Remove existing roles
                user.UserRoles.Clear();
                await _userRepository.SaveAsync();

                // Assign new roles
                foreach (var roleId in updateUserDto.RoleIds)
                {
                    await AssignRoleToUserAsync(user.Id, roleId);
                }
            }

            return await GetUserByIdAsync(user.Id) ?? new UserDto();
        }

        public async Task<bool> DeleteUserAsync(long id)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(id);
            if (user == null) return false;

            _userRepository.Delete(id);
            await _userRepository.SaveAsync();
            return true;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByUsernameOrEmailAsync(loginDto.UsernameOrEmail ?? string.Empty);

            if (user == null)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid username/email or password"
                };
            }

            if (user.IsLocked)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Account is locked. Please contact administrator."
                };
            }

            if (!user.IsActive)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Account is not active"
                };
            }

            // Verify password
            if (!_passwordHasher.VerifyPassword(loginDto.Password ?? string.Empty, user.PasswordHash ?? string.Empty, user.PasswordSalt ?? string.Empty))
            {
                user.FailedLoginAttempts++;
                
                // Lock account after 5 failed attempts
                if (user.FailedLoginAttempts >= 5)
                {
                    user.IsLocked = true;
                }

                _userRepository.Update(user);
                await _userRepository.SaveAsync();

                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid username/email or password"
                };
            }

            // Successful login
            user.LastLoginAt = DateTime.UtcNow;
            user.FailedLoginAttempts = 0;
            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            var userDto = await GetUserByIdAsync(user.Id);

            return new LoginResponseDto
            {
                Success = true,
                Message = "Login successful",
                User = userDto,
                Token = GenerateToken(user),
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

        public async Task<bool> LogoutAsync(long userId)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            if (user == null) return false;

            return true;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(changePasswordDto.UserId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            if (!_passwordHasher.VerifyPassword(
                changePasswordDto.CurrentPassword ?? string.Empty, 
                user.PasswordHash ?? string.Empty, 
                user.PasswordSalt ?? string.Empty))
            {
                throw new InvalidOperationException("Current password is incorrect");
            }

            var (passwordHash, passwordSalt) = _passwordHasher.HashPassword(changePasswordDto.NewPassword ?? string.Empty);
            
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordChangedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            return true;
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string roleName)
        {
            var users = await _userRepository.GetUsersByRoleAsync(GetRoleIdByName(roleName));
            return users.Select(u =>
            {
                var userDto = _mapper.Map<UserDto>(u);
                userDto.Roles = u.UserRoles.Select(ur => ur.Role?.RoleName ?? string.Empty).ToList();
                return userDto;
            });
        }

        public async Task<bool> AssignRoleToUserAsync(long userId, long roleId)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            if (user == null) return false;

            if (user.UserRoles.Any(ur => ur.RoleId == roleId))
                return true;

            var userRole = new UserRoleEntity
            {
                UserId = userId,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow
            };

            user.UserRoles.Add(userRole);
            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            return true;
        }

        public async Task<bool> RemoveRoleFromUserAsync(long userId, long roleId)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            if (user == null) return false;

            var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == roleId);
            if (userRole == null) return false;

            user.UserRoles.Remove(userRole);
            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            return true;
        }

        public async Task<bool> LockUserAsync(long userId)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            if (user == null) return false;

            user.IsLocked = true;
            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            return true;
        }

        public async Task<bool> UnlockUserAsync(long userId)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            if (user == null) return false;

            user.IsLocked = false;
            user.FailedLoginAttempts = 0;
            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            return true;
        }

        private string GenerateToken(UserEntity user)
        {
            // TODO: Implement JWT token generation
            return "TOKEN_PLACEHOLDER";
        }

        private long GetRoleIdByName(string roleName)
        {
            return roleName.ToLower() switch
            {
                "admin" => 1,
                "doctor" => 2,
                "patient" => 3,
                _ => 3
            };
        }
    }
}