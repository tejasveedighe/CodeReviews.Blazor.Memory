using System.ComponentModel.DataAnnotations;
using MemoryGame.Shared.Models;
using SQLite;

namespace MemoryGame.Services;

public sealed class HistoryItem
{
    [Required]
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string UserName { get; set; }

    [Required]
    public int Score { get; set; }

    [Required]
    public GameDifficulty Difficulty { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime GameStartedTime { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime GameEndedTime { get; set; }
}
