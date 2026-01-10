using Doctor.Appointment.Domain.DTOs.User;
using System.Net.Http.Json;

namespace Doctor.Appointment.Web.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001");
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/users/login", loginDto);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LoginResponseDto>() ?? new LoginResponseDto { Success = false };
                }

                var errorContent = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return new LoginResponseDto 
                { 
                    Success = false, 
                    Message = errorContent?.Message ?? "Login failed" 
                };
            }
            catch (Exception ex)
            {
                return new LoginResponseDto 
                { 
                    Success = false, 
                    Message = $"An error occurred: {ex.Message}" 
                };
            }
        }

        public async Task<(bool Success, string Message)> RegisterAsync(CreateUserDto createUserDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/users", createUserDto);
                
                if (response.IsSuccessStatusCode)
                {
                    return (true, "Registration successful");
                }

                var errorContent = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return (false, errorContent?.Message ?? "Registration failed");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<bool> LogoutAsync(long userId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/users/logout/{userId}", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private class ErrorResponse
        {
            public string? Message { get; set; }
        }
    }
}