using MauiAppPeliculas.Repositories;
using MauiAppPeliculas.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using System;
using System.Net.Http;
using System.Diagnostics;

namespace MauiAppPeliculas
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // 1) URL de producción (Somee) como ÚNICA base
            var baseUrl = "https://localhost:7179".TrimEnd('/');

            // 2) Limpiar preferencia vieja que quedó apuntando a localhost (una vez basta)
            try { Preferences.Remove("ApiBaseUrl"); } catch { }

            // 3) HttpClient nombrado con BaseAddress
            builder.Services.AddHttpClient("api", c =>
            {
                c.BaseAddress = new Uri(baseUrl);
            });

            // 4) Servicios como SINGLETON usando ese HttpClient
            builder.Services.AddSingleton<UsuarioDtoServiceHttp>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("api");
                return new UsuarioDtoServiceHttp(client);
            });

            builder.Services.AddSingleton<PeliculasDtoServiceHttp>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("api");
                return new PeliculasDtoServiceHttp(client);
            });

            // 5) Repositorios (pueden ser Scoped)
            builder.Services.AddScoped<PeliculasRepositoryHttp>();
            builder.Services.AddScoped<UsuariosRepositoryHttp>();

            return builder.Build();
        }
    }
}
