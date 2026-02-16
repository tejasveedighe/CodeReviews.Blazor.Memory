namespace MemoryGame.Shared.Models;

public sealed class HistoryItemDto
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public int Score { get; set; }
    public GameDifficulty Difficulty { get; set; }
    public DateTime GameStartedTime { get; set; }
    public DateTime GameEndedTime { get; set; }
}
