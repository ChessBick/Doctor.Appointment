using Doctor.Appointment.Domain.DTOs.User;
using Doctor.Appointment.Web.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Doctor.Appointment.Web.Components.Pages.Auth
{
    public partial class Register
    {
        [Inject]
        private AuthService AuthService { get; set; } = default!;
        [Inject]
        private ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;
        private CreateUserDto model = new() { RoleIds = new List<long> { 2 } }; // Default to Patient role (ID: 2)
        private bool passwordVisible = false;
        private bool acceptTerms = false;
        private bool isLoading = false;
        private InputType passwordInput = InputType.Password;
        private string passwordIcon = Icons.Material.Filled.VisibilityOff;

        private void TogglePasswordVisibility()
        {
            passwordVisible = !passwordVisible;
            passwordInput = passwordVisible ? InputType.Text : InputType.Password;
            passwordIcon = passwordVisible ? Icons.Material.Filled.Visibility : Icons.Material.Filled.VisibilityOff;
        }

        private async Task HandleRegistration()
        {
            if (!acceptTerms)
            {
                Snackbar.Add("You must accept the terms and conditions", Severity.Warning);
                return;
            }

            if (string.IsNullOrEmpty(model.Password) || model.Password.Length < 8)
            {
                Snackbar.Add("Password must be at least 8 characters", Severity.Warning);
                return;
            }

            isLoading = true;

            try
            {
                var (success, message) = await AuthService.RegisterAsync(model);

                if (success)
                {
                    Snackbar.Add("Registration successful! Please login.", Severity.Success);
                    Navigation.NavigateTo("/login");
                }
                else
                {
                    Snackbar.Add(message, Severity.Error);
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
