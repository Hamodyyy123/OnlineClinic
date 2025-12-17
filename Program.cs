using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OnlineClinic.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Add your OnlineClinicContext to the DI container.
// Make sure "DefaultConnection" exists in appsettings.json.
builder.Services.AddDbContext<OnlineClinicContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// OPTIONAL: add authorization services explicitly (useful if you add policies later)
builder.Services.AddAuthorization();

// Cookie authentication (scheme name "Cookies")
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        // Path the middleware will redirect to for login
        options.LoginPath = "/Login/Index";
        options.LogoutPath = "/Login/Logout";

        // A few useful cookie options you may want to tune:
        options.Cookie.Name = "OnlineClinic.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        // options.AccessDeniedPath = "/Account/AccessDenied"; // if you plan to show a 403 page
    });

// Build app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Detailed errors in development
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

// Optional: return friendly pages for 401/403, static files etc.
// app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();