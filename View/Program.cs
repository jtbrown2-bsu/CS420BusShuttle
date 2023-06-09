using Core;
using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var folder = Environment.SpecialFolder.LocalApplicationData;
var path = Environment.GetFolderPath(folder);
var DbPath = Path.Join(path, "shuttle.db");

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddDbContext<ShuttleDbContext>(options => options.UseInMemoryDatabase("CS420Bus"));
builder.Services.AddDbContext<ShuttleDbContext>(options => options.UseSqlite($"Data Source={DbPath}"));
builder.Services.AddScoped<IBusRepository, BusRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IEntryRepository, EntryRepository>();
builder.Services.AddScoped<ILoopRepository, LoopRepository>();
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<IStopRepository, StopRepository>();

builder.Services.AddDefaultIdentity<Driver>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ShuttleDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagerOnly", policy =>
        policy.RequireClaim("IsManager", "true"));
    options.AddPolicy("ActivatedOnly", policy =>
        policy.RequireClaim("IsActivated", "true"));
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/User/Login";
});

var app = builder.Build();

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

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();