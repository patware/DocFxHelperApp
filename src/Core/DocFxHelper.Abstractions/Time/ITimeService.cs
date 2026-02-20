namespace DocFxHelper.Abstractions.Time
{
  public interface ITimeService
  {
    DateTimeOffset UtcNow { get; }
  }
}
