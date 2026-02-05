using Blazored.SessionStorage;
using Doctor.Appointment.Domain.DTOs.User;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace Doctor.Appointment.Web.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorage;
        private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private const string USER_SESSION_KEY = "CurrentUser";

        public CustomAuthenticationStateProvider(ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var user = await _sessionStorage.GetItemAsync<UserDto>(USER_SESSION_KEY);
                
                if (user == null)
                {
                    return new AuthenticationState(_anonymous);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
                };

                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var identity = new ClaimsIdentity(claims, "SessionAuth");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                return new AuthenticationState(claimsPrincipal);
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task MarkUserAsAuthenticatedAsync(UserDto user)
        {
            await _sessionStorage.SetItemAsync(USER_SESSION_KEY, user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, "SessionAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task MarkUserAsLoggedOutAsync()
        {
            await _sessionStorage.RemoveItemAsync(USER_SESSION_KEY);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            try
            {
                return await _sessionStorage.GetItemAsync<UserDto>(USER_SESSION_KEY);
            }
            catch
            {
                return null;
            }
        }
    }
}