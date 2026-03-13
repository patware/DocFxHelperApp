namespace DocFxHelper.Infrastructure
{
  public interface ITimeService
  {
    DateTimeOffset UtcNow { get; }
  }
}
