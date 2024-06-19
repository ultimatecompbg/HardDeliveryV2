using HardDelivery.Data;
using HardDelivery.Models;
using HardDelivery.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(Configuration.ConnectionString));

// Configure Identity with custom User and Role types
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();

// Register DeliveryService as IDeliveryService
builder.Services.AddScoped<IDeliveryService, DeliveryService>();

// Register AdminService as IAdminService
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Migrate and seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
    SeedData(services).Wait(); // Ensure SeedData method completes before moving on
}

// Development environment specific configurations
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else // Production environment configurations
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map areas, controllers, and Razor pages
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

// SeedData method to create roles if they do not exist
async Task SeedData(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

    string adminRole = "admin";
    string courierRole = "courier";

    // Check if the roles exist, and create them if they don't
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole<int>(adminRole));
    }

    if (!await roleManager.RoleExistsAsync(courierRole))
    {
        await roleManager.CreateAsync(new IdentityRole<int>(courierRole));
    }
}
