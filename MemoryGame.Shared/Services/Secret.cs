namespace MemoryGame.Shared.Services;

public interface ISecret
{
    public Guid Id { get; set; }
    public Type Type { get; set; }
    public object Value { get; set; }
    public bool Visible { get; set; }
    public HightlightType HightlightType { get; set; }
}

public class Secret : ISecret
{
    public Guid Id { get; set; }
    public Type Type { get; set; }
    public object Value { get; set; }
    public bool Visible { get; set; } = false;
    public HightlightType HightlightType { get; set; } = HightlightType.Simple;
}

public enum HightlightType
{
    Simple,
    Match
}