
namespace DocFxHelper.Infrastructure
{
  public sealed class SystemTimeService : ITimeService
  {
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
  }
}
