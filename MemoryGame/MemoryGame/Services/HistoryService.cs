using MemoryGame.Shared.Stores;
using SQLite;

namespace MemoryGame.Services;

public sealed class HistoryService
{
    SQLiteConnection _dbConnection;

    public HistoryService()
    {
        _dbConnection = new("history.db");
        _dbConnection.CreateTable<HistoryItem>();
    }

    public async Task AddHistoryItem(HistoryItem item)
    {
        if (item is null)
            throw new InvalidOperationException("Cannot add null history");

        _dbConnection.Insert(item);
    }

    public List<HistoryItem> GetHistory()
    {
        var data = _dbConnection.Table<HistoryItem>().ToList();
        return data;
    }
}
