using MemoryGame.Shared.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace MemoryGame.Client
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddScoped(s =>
            {
                var webRootHost = s.GetRequiredService<IWebAssemblyHostEnvironment>();
                return new HttpClient() { BaseAddress = new Uri(webRootHost.BaseAddress) };
            });
            builder.Services.AddSharedServices();
            builder.Services.AddLogging();

            await builder.Build().RunAsync();
        }
    }
}
