namespace MemoryGame.Shared.Services;

public class InvalidSecretCountRequestedException : Exception
{
    public InvalidSecretCountRequestedException(string? message)
        : base(message) { }
}
