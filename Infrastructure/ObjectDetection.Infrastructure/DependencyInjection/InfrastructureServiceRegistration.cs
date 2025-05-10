using Microsoft.Extensions.DependencyInjection;
using ObjectDetection.Application.Abstractions;
using ObjectDetection.Application.UseCases;
using ObjectDetection.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetection.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddHttpClient<IFlaskAIClient, FlaskAIClient>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IAnalyzeImageUseCase, AnalyzeImageUseCase>();

            return services;
        }
    }
}
