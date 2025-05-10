using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ObjectDetection.Persistance.DependencyInjection;
using ObjectDetection.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// ?? Add services to the container
builder.Services.AddPersistenceServices(builder.Configuration)
    .AddInfrastructureServices(); // bunu ekle

builder.Services.AddControllersWithViews(); // MVC için

// ? Authentication ve Authorization middleware'leri için gerekli
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// ?? HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ?? Authentication ve Authorization
app.UseAuthentication();
app.UseAuthorization();

// ?? MVC Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
