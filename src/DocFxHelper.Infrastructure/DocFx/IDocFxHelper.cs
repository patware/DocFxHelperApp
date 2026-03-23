namespace DocFxHelper.Infrastructure.DocFx
{
  public interface IDocFxHelper
  {
    Task<DocFxInstallationMetadata> GetInstallMetadataAsync(CancellationToken ct = default);
    Task<int> RunDocfxMetadataAsync(string workingDirectory, CancellationToken ct = default);
  }
}