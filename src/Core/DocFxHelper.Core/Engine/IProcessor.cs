using DocFxHelper.Domain;

namespace DocFxHelper.Core.Engine
{
  public interface IProcessor
  {
    Task ProcessAsync(Run run, CancellationToken ct = default!);
  }
}