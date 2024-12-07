using CUOIKY.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure the DbContext to use SQL Server
builder.Services.AddDbContext<LTWDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session expiration time
    options.Cookie.HttpOnly = true;                 // Make session cookie accessible only via HTTP
    options.Cookie.IsEssential = true;              // Ensure session cookie is considered essential
});

// Add logging and antiforgery configuration
builder.Services.AddLogging();
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Ensure HSTS is used in production
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();       // Enable session before authorization
app.UseAuthorization(); // Enable authorization if required

// Set up the default route for MVC controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
