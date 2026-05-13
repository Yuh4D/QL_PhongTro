using WebQLPT.Data;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";

        options.AccessDeniedPath = "/Account/AccessDenied";

        options.ExpireTimeSpan = TimeSpan.FromDays(7);

        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();


builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

var env = app.Environment;
RotativaConfiguration.Setup(env.WebRootPath, "Rotativa");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
