using Doctor.Appointment.Domain.DTOs.User;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace Doctor.Appointment.Web.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    return new AuthenticationState(_anonymous);
                }

                var userJson = httpContext.Session.GetString("CurrentUser");
                if (string.IsNullOrEmpty(userJson))
                {
                    return new AuthenticationState(_anonymous);
                }

                var user = JsonSerializer.Deserialize<UserDto>(userJson);
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

                var identity = new ClaimsIdentity(claims, "CustomAuth");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                return new AuthenticationState(claimsPrincipal);
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }

        public void MarkUserAsAuthenticated(UserDto user)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var userJson = JsonSerializer.Serialize(user);
                httpContext.Session.SetString("CurrentUser", userJson);

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

                var identity = new ClaimsIdentity(claims, "CustomAuth");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
            }
        }

        public void MarkUserAsLoggedOut()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                httpContext.Session.Remove("CurrentUser");
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
            }
        }

        public UserDto? GetCurrentUser()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null) return null;

                var userJson = httpContext.Session.GetString("CurrentUser");
                if (string.IsNullOrEmpty(userJson)) return null;

                return JsonSerializer.Deserialize<UserDto>(userJson);
            }
            catch
            {
                return null;
            }
        }
    }
}