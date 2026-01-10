using Doctor.Appointment.Web.Components;
using Doctor.Appointment.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();

// Add Session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add HttpClient for API calls
builder.Services.AddHttpClient<AuthService>();

// Add Authentication
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.LoginPath = "/login";
    options.SlidingExpiration = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// Add Session middleware
app.UseSession();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
