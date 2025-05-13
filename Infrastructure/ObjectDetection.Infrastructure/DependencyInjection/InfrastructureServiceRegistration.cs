using Microsoft.Extensions.DependencyInjection;
using Nest;
using ObjectDetection.Application.Abstractions;
using ObjectDetection.Application.UseCases;
using ObjectDetection.Infrastructure.Service;
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
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                            .DefaultIndex("images");

            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);
            services.AddScoped<IElasticImageService, ElasticImageService>();
            services.AddScoped<ISearchImageUseCase, SearchImageService>();


            return services;
        }
    }
}
