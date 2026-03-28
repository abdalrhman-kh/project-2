using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project_2.Data;
using project_2.Helpers.Email;
using project_2.Helpers.ImageHelpers;
using project_2.Helpers.OTPHelper;
using project_2.interceptors;
using project_2.Models.Repositories;

var builder = WebApplication.CreateBuilder(args);
var connect = builder.Configuration.GetConnectionString("Connect");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DataDbContext>(x => x.UseSqlServer(connect).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).AddInterceptors(new SoftDelete()));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IImageHelper, ImageHelper>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IOTPHelper, OTPHelper>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<DataDbContext>();
builder.Services.ConfigureApplicationCookie(x=> {
    x.LoginPath = "/Admin/Account/Login";
    x.LogoutPath = "/Home/Index";
    x.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    x.SlidingExpiration = true;
    });

builder.Services.Configure<IdentityOptions>(x =>
{
    x.Password.RequiredUniqueChars = 0;
    x.Password.RequireDigit = true;
    x.Password.RequireLowercase = false;
    x.Password.RequireUppercase = false;
    x.Password.RequiredLength = 8;
    x.Password.RequireNonAlphanumeric = false;
}
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
