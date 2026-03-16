using DocFxHelper.Abstractions.Engine;

namespace DocFxHelper.Core.Utils
{
  public interface IGeneral
  {
    void EnsureExists(BuildPaths buildPaths);

    void MoveToOrphanFolder(string folderPath, BuildPaths buildPaths);
  }
}