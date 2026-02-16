using MemoryGame.Shared.Services;
using MemoryGame.Shared.Stores;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace MemoryGame.Shared.Extensions
{
    public static class ShareServicesExtensions
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services)
        {
            services.AddMudServices();
            services.AddScoped<HistoryApiService>();
            services.AddScoped<GameStore>();
            services.AddScoped<SecretService>();
            return services;
        }
    }
}
