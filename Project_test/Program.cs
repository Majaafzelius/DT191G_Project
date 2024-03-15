using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_test.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Project_testContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Project_testContext")
    ?? throw new InvalidOperationException("Connection string 'Project_testContext' not found.")));

// Add Identity services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // Adding roles to Identity
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Adding authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB limit, adjust as needed
});

var app = builder.Build();

// Retrieve services from the service provider
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

var context = services.GetRequiredService<ApplicationDbContext>();
var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

// Ensure the database is created and migrated
context.Database.Migrate();

// Create Roles if they don't exist
if (!await roleManager.RoleExistsAsync("Admin"))
{
    await roleManager.CreateAsync(new IdentityRole("Admin"));
}

// Create a sample admin user if not exists
var adminUser = await userManager.FindByNameAsync("admin@example.com");
if (adminUser == null)
{
    adminUser = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com" };
    await userManager.CreateAsync(adminUser, "Password123!"); // Change password as needed
    await userManager.AddToRoleAsync(adminUser, "Admin");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Add this line to enable authentication
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Categories}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
