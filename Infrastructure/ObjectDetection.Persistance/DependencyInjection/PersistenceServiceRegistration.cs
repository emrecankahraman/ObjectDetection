using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ObjectDetection.Domain.Entities;
using ObjectDetection.Persistance.Context;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ObjectDetection.Application.Abstractions; 
using ObjectDetection.Persistance.Concretes;
using ObjectDetection.Application.UseCases;


namespace ObjectDetection.Persistance.DependencyInjection
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext ayarı
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Identity ayarı - AppUser ve AppRole Guid ile çalışacak şekilde
            services.TryAddScoped<IUserStore<AppUser>, UserOnlyStore<AppUser, ApplicationDbContext, Guid>>();
            services.TryAddScoped<IRoleStore<AppRole>, RoleStore<AppRole, ApplicationDbContext, Guid>>();

            services.AddIdentity<AppUser,AppRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
            })
                .AddRoles<AppRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<IAnalyzeImageUseCase, AnalyzeImageUseCase>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddHttpContextAccessor(); // SignInManager için gerekli

            services.AddScoped<IGetUserImagesUseCase, GetUserImagesUseCase>();


            return services;
        }
    }
}
