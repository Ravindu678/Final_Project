using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookIt1.Data;
using BookIt1.Areas.Identity.Data;
using System.Configuration;
using Stripe;
using BookIt1.Models;
//using BookIt1.Services.Background;
using BookIt1.Services;
using BookIt1.Services.Recommendation;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("BookIt1DbContextConnection") ?? throw new InvalidOperationException("Connection string 'BookIt1DbContextConnection' not found.");

builder.Services.AddDbContext<BookIt1DbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<BookIt1User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>() // Enable roles
.AddEntityFrameworkStores<BookIt1DbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddHostedService<BookingExportService>();
builder.Services.AddScoped<BookingDataExporter>();
builder.Services.AddScoped<RecommendationDataService>();

builder.Services.Configure<StripeSettings>(
    builder.Configuration.GetSection("Stripe"));

builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
});

builder.Services.AddSession(); // Enable Session
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Ensure database is migrated and roles are created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<BookIt1DbContext>();
    var userManager = services.GetRequiredService<UserManager<BookIt1User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // Use Session Middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Account}/{action=Login}/{id?}"
);

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=AdminDashboard}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Event}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();


var stripeSettings = builder.Configuration.GetSection("Stripe").Get<StripeSettings>();
Stripe.StripeConfiguration.ApiKey = stripeSettings.SecretKey;


app.Run();
