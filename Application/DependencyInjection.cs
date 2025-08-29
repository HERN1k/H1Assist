using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IImageEditorService, ImageEditorService>();
            services.AddScoped<ISeoGeneratorService, SeoGeneratorService>();
            services.AddScoped<IDescriptionService, DescriptionService>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddScoped<IApiService, ApiService>();
            services.AddScoped<IHtmlManagerService, HtmlManagerService>();
            services.AddScoped<IIcecatService, IcecatService>();

            return services;
        }
    }
}