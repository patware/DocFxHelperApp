namespace DocFxHelper.Core.Drops
{
  public interface IScout
  {
    IReadOnlyList<System.IO.DirectoryInfo> Recon(string drop);
  }
}