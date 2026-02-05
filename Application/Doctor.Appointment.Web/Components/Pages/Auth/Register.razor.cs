using Doctor.Appointment.Domain.DTOs.User;
using Doctor.Appointment.Web.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

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
        
        private MudForm form = default!;
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

        private Func<object, string, Task<IEnumerable<string>>> ValidateModel => async (model, propertyName) =>
        {
            var errors = new List<string>();
            
            if (propertyName == nameof(CreateUserDto.Password))
            {
                if (string.IsNullOrEmpty(this.model.Password))
                {
                    errors.Add("Password is required");
                }
                else if (this.model.Password.Length < 8)
                {
                    errors.Add("Password must be at least 8 characters");
                }
            }

            return errors;
        };

        private async Task HandleRegistration()
        {
            await form.Validate();

            if (!form.IsValid)
            {
                Snackbar.Add("Please correct the errors in the form", Severity.Warning);
                return;
            }

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
