using Doctor.Appointment.Domain.DTOs.User;
using Doctor.Appointment.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;

namespace Doctor.Appointment.Web.Components.Pages.Auth
{
    public partial class Login
    {
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private CustomAuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        private LoginDto model = new();
        private bool passwordVisible = false;
        private bool rememberMe = false;
        private bool isLoading = false;
        private InputType passwordInput = InputType.Password;
        private string passwordIcon = Icons.Material.Filled.VisibilityOff;
        private string? returnUrl;

        protected override void OnInitialized()
        {
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("returnUrl", out var url))
            {
                returnUrl = url;
            }
        }

        private void TogglePasswordVisibility()
        {
            passwordVisible = !passwordVisible;
            passwordInput = passwordVisible ? InputType.Text : InputType.Password;
            passwordIcon = passwordVisible ? Icons.Material.Filled.Visibility : Icons.Material.Filled.VisibilityOff;
        }

        private async Task HandleLogin()
        {
            isLoading = true;

            try
            {
                var response = await AuthService.LoginAsync(model);

                if (response.Success && response.User != null)
                {
                    AuthStateProvider.MarkUserAsAuthenticated(response.User);
                    Snackbar.Add("Login successful!", Severity.Success);

                    // Redirect to return URL or role-based dashboard
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        Navigation.NavigateTo(returnUrl, forceLoad: true);
                    }
                    else if (response.User.Roles.Contains("Admin"))
                    {
                        Navigation.NavigateTo("/admin");
                    }
                    else if (response.User.Roles.Contains("Doctor"))
                    {
                        Navigation.NavigateTo("/doctor/dashboard");
                    }
                    else if (response.User.Roles.Contains("Patient"))
                    {
                        Navigation.NavigateTo("/patient/dashboard");
                    }
                    else
                    {
                        Navigation.NavigateTo("/");
                    }
                }
                else
                {
                    Snackbar.Add(response.Message ?? "Login failed", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"An error occurred: {ex.Message}", Severity.Error);
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}
