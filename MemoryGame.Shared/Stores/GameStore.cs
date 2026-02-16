using System.Timers;
using MemoryGame.Shared.Models;
using MemoryGame.Shared.Services;
using Microsoft.Extensions.Logging;

namespace MemoryGame.Shared.Stores;

public sealed class GameStore
{
    public GameStore(
        SecretService secretService,
        ILogger<GameStore> logger,
        HistoryApiService historyService
    )
    {
        SecretService = secretService;
        Logger = logger;
        HistoryService = historyService;
        Timer = new System.Timers.Timer(1000);
        Timer.Enabled = true;
        Timer.Elapsed += (sender, args) =>
        {
            Seconds--;

            if (Minutes == 0 && Seconds == 0)
            {
                SetGamehasEnded();
            }
            else if (Seconds == 0)
            {
                Seconds = 59;
                Minutes--;
            }
            AppEvents.Instance.InvokeTimeUpdate();
        };

        AppEvents.Instance.UserNameUpdated += UpdateUserName;
        AppEvents.Instance.CardClicked += HandleCardClick;

        Logger.LogInformation("Game Store Created");
    }

    public bool GameHasStarted { get; private set; } = false;
    public bool IsGameOver { get; private set; } = true;
    public int Minutes { get; private set; }
    public int Seconds { get; private set; }
    private System.Timers.Timer Timer { get; set; }
    private HistoryItemDto? HistoryItem { get; set; }

    public void StartGame()
    {
        HistoryItem = new HistoryItemDto
        {
            UserName = UserName,
            Difficulty = SelectedDifficulty,
            GameStartedTime = DateTime.Now,
        };

        IsGameOver = false;
        GameHasStarted = true;

        Score = 0;

        Minutes = _timers[SelectedDifficulty];

        Seconds = 60;

        Timer.Start();

        AppEvents.Instance.InvokeStoreHasUpdated();
    }

    private async void SetGamehasEnded()
    {
        if (!_scoreUpdating.IsCompleted)
        {
            await _scoreUpdating;
        }

        HistoryItem!.GameEndedTime = DateTime.Now;
        HistoryItem!.Score = Score;
        bool itemAdded = await HistoryService.AddHistoryItemAsync(HistoryItem);

        HistoryItem = null;
        GameHasStarted = false;
        IsGameOver = true;
        Timer.Stop();
        AppEvents.Instance.InvokeStoreHasUpdated();
        AppEvents.Instance.InvokeGameOver();
    }

    ~GameStore()
    {
        Timer.Stop();
        Timer.Close();
        Timer.Dispose();
        AppEvents.Instance.UserNameUpdated -= UpdateUserName;
        AppEvents.Instance.CardClicked -= HandleCardClick;

        Logger.LogInformation("Game Store Destroyed");
    }

    public int Score { get; private set; }

    private Task _scoreUpdating;

    private void HandleCardClick(Guid secretId)
    {
        int revealCount = _secrets.Count(x => x.Visible);

        if (revealCount > 1)
        {
            return;
        }

        ISecret revealSecret = _secrets.Single(x => x.Id.CompareTo(secretId) == 0);
        revealSecret.Visible = true;
        AppEvents.Instance.InvokeSecretsUpdate();

        ISecret? alreadyRevealedSecret = _secrets.SingleOrDefault(x =>
            x.Visible && x.Id.CompareTo(secretId) != 0
        );
        if (alreadyRevealedSecret is not null && alreadyRevealedSecret.IsEqual(revealSecret))
        {
            alreadyRevealedSecret.HightlightType = HightlightType.Match;
            revealSecret.HightlightType = HightlightType.Match;
            AppEvents.Instance.InvokeSecretsUpdate();

            _scoreUpdating = Task.Run(async () =>
            {
                await Task.Delay(1000);

                _secrets.Remove(alreadyRevealedSecret);
                _secrets.Remove(revealSecret);
                AppEvents.Instance.InvokeSecretsUpdate();

                IncreaseScore();
            });

            // game has ended, since the last two are remining
            if (_secrets.Count == 2)
            {
                SetGamehasEnded();
            }
        }
        else
        {
            if (revealCount > 0)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(500);
                    HideAllSecrets();
                });
            }
        }
    }

    private void IncreaseScore()
    {
        Score++;
        AppEvents.Instance.InvokeScoreUpdate();
    }

    private void HideAllSecrets()
    {
        _secrets.ForEach(x => x.Visible = false);
        AppEvents.Instance.InvokeSecretsUpdate();
    }

    #region Username

    public string UserName { get; private set; } = string.Empty;

    private void UpdateUserName(string name) => UserName = name;

    #endregion Username

    #region Game Difficulty

    private readonly Dictionary<GameDifficulty, int> _timers = new()
    {
        { GameDifficulty.Easy, 4 },
        { GameDifficulty.Medium, 3 },
        { GameDifficulty.Hard, 1 },
    };

    public GameDifficulty SelectedDifficulty { get; private set; }
    public SecretService SecretService { get; }
    public ILogger<GameStore> Logger { get; }
    public HistoryApiService HistoryService { get; }

    private List<ISecret> _secrets = [];
    public IReadOnlyList<ISecret> Secrets => _secrets.AsReadOnly();

    public async Task UpdateGameDifficulty(GameDifficulty difficulty)
    {
        if (Enum.TryParse(typeof(GameDifficulty), difficulty.ToString(), out _))
        {
            SelectedDifficulty = difficulty;

            await GenerateSecrets();

            AppEvents.Instance.InvokeStoreHasUpdated();
        }
    }

    private async Task GenerateSecrets()
    {
        _secrets = await SecretService.GetSecrets(
            SelectedDifficulty switch
            {
                GameDifficulty.Easy => 4,
                GameDifficulty.Medium => 16,
                GameDifficulty.Hard => 32,
                _ => 4,
            }
        );
    }

    #endregion Game Difficulty
}
