using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Doctor.Appointment.Web.Services
{
    public class BlazorAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BlazorAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // This handler doesn't actually authenticate - that's done by AuthenticationStateProvider
            // It's just here to satisfy the requirement for IAuthenticationService
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            // Redirect to login page when authorization fails
            Response.Redirect("/login");
            return Task.CompletedTask;
        }
    }
}