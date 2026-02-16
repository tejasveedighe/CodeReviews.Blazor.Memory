using MemoryGame.Components;
using MemoryGame.Services;
using MemoryGame.Shared.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace MemoryGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder
                .Services.AddRazorComponents(options => options.DetailedErrors = true)
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped(s =>
            {
                var contextAccessor = s.GetRequiredService<IHttpContextAccessor>();
                var request = contextAccessor.HttpContext?.Request;
                var baseUri = $"{request.Scheme}://{request.Host}{request.PathBase}/";
                return new HttpClient() { BaseAddress = new Uri("http://localhost:5184/") };
            });

            builder.Services.AddScoped<HistoryService>();
            builder.Services.AddSharedServices();
            builder.Services.AddLogging();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            app.MapGet(
                "/getHistory",
                (
                    [FromServices] HistoryService historyService,
                    [FromServices] ILogger<Program> logger
                ) =>
                {
                    try
                    {
                        logger.LogInformation("Get History");
                        var data = historyService.GetHistory();
                        logger.LogInformation($"History returned {data.Count} items");
                        return Results.Ok(data);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                        logger.LogError(ex.StackTrace);
                        return Results.Ok(new List<HistoryItem>());
                    }
                }
            );

            app.MapPost(
                "/addHistory",
                async (
                    [FromServices] HistoryService historyService,
                    [FromServices] ILogger<Program> logger,
                    HistoryItem item
                ) =>
                {
                    try
                    {
                        await historyService.AddHistoryItem(item);
                        logger.LogInformation("Added a new item");
                        return Results.Ok("Added to db");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                        logger.LogError(ex.StackTrace);
                        return Results.InternalServerError();
                    }
                }
            );

            app.MapGet(
                "/getImages",
                async ([FromServices] ILogger<Program> logger, int count = 4) =>
                {
                    try
                    {
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "Images");
                        var directoryInfo = new DirectoryInfo(path);
                        List<string> images = [];
                        foreach (var file in directoryInfo.GetFiles().Take(count))
                        {
                            var ms = new MemoryStream();
                            await file.OpenRead().CopyToAsync(ms);
                            images.Add(Convert.ToBase64String(ms.ToArray()));
                        }
                        logger.LogInformation($"Server found {images.Count} images");
                        return Results.Ok(images);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                        logger.LogError(ex.StackTrace);
                        return Results.InternalServerError();
                    }
                }
            );

            app.Run();
        }
    }
}
