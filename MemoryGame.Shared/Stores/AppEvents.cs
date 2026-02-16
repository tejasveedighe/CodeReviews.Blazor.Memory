namespace MemoryGame.Shared.Stores;

public sealed class AppEvents
{
    public static AppEvents Instance { get; } = new();

    public event Action<string>? UserNameUpdated;

    public void InvokeUserNameUpdated(string name)
    {
        UserNameUpdated?.Invoke(name);
    }

    public event Action? StoreHasUpdated;

    public void InvokeStoreHasUpdated()
    {
        StoreHasUpdated?.Invoke();
    }

    public event Action<Guid>? CardClicked;

    public void PublishCardClick(Guid id)
    {
        CardClicked?.Invoke(id);
    }

    public event Action? TimeUpdated;

    public void InvokeTimeUpdate()
    {
        TimeUpdated?.Invoke();
    }

    public event Action? ScoreUpdated;

    public void InvokeScoreUpdate()
    {
        ScoreUpdated?.Invoke();
    }

    public event Action? SecretsUpdated;

    public void InvokeSecretsUpdate() => SecretsUpdated?.Invoke();

    public event Action? GameOver;

    public void InvokeGameOver() => GameOver?.Invoke();
}
