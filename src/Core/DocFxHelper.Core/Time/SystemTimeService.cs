using DocFxHelper.Abstractions.Time;

namespace DocFxHelper.Core.Time
{
  public sealed class SystemTimeService : ITimeService
  {
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
  }
}
