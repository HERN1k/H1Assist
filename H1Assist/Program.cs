using System.Globalization;

using Application;

using Infrastructure;

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;

namespace H1Assist
{
    public class Program
    {
        public static readonly string[] SupportedCultures = new[] { "en", "uk" };

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            builder.Services.AddLocalization(options => options.ResourcesPath = "Localization");

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure();

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = SupportedCultures.Select(c => new CultureInfo(c)).ToList(),
                SupportedUICultures = SupportedCultures.Select(c => new CultureInfo(c)).ToList()
            };

            requestLocalizationOptions.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());

            app.UseRequestLocalization(requestLocalizationOptions);

            app.UseForwardedHeaders();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}