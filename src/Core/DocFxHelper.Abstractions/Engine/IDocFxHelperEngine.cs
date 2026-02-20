namespace DocFxHelper.Abstractions.Engine
{
  public interface IDocFxHelperEngine
  {
    Task<BuildResult> BuildAsync(
        BuildRequest request,
        CancellationToken cancellationToken = default);
  }
}
