using System.Globalization;

using Application;
using Application.Interfaces;
using Application.Services;

using Domain.ValueObjects;

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

            builder.Services.AddMemoryCache();

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure();

            builder.Services.AddHttpClient(nameof(Application));
            builder.Services.AddHttpClient(nameof(IHtmlManagerService), client => 
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue() { NoCache = true };
            });
            builder.Services.AddHttpClient(nameof(IIcecatService), client =>
            {
                // TEMP
                client.DefaultRequestHeaders.Add("api_token", "dfca579f-e43c-4120-a247-c324373f85c4");
                client.DefaultRequestHeaders.Add("content_token", "5baebb25-2f4d-4321-a515-f4218bbfc0d5");
            });

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