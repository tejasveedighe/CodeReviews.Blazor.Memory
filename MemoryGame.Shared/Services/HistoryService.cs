using System.Net.Http.Json;
using MemoryGame.Shared.Models;
using Microsoft.Extensions.Logging;

namespace MemoryGame.Shared.Services;

public class HistoryApiService(HttpClient httpClient, ILogger<HistoryApiService> logger)
{
    public async Task<List<HistoryItemDto>> GetHistoryItemsAsync()
    {
        try
        {
            var res = await httpClient.GetFromJsonAsync<List<HistoryItemDto>>("/getHistory");
            return res!;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            logger.LogError(ex.StackTrace);
            return [];
        }
    }

    public async Task<bool> AddHistoryItemAsync(HistoryItemDto item)
    {
        try
        {
            var res = await httpClient.PostAsJsonAsync<HistoryItemDto>("/addHistory", item);
            return res.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            logger.LogError(ex.StackTrace);
            return false;
        }
    }
}
