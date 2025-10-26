using BlazorLab.Components;
using BlazorLab.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace BlazorLab
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure logging to use only Console
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Services.Configure<DownloaderOptions>(builder.Configuration.GetSection("DownloaderOptions"));

            // Register the download service for DI
            builder.Services.AddScoped<IDownloadService, DownloadService>();
            builder.Services.AddSingleton<DownloadEventNotifier>();
            builder.Services.AddSingleton<IVideoFileService, VideoFileService>();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "media"));

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(AppContext.BaseDirectory, "media")),
                RequestPath = "/downloads"
            });

            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
