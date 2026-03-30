using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Specs
{
  public sealed record BuildSpec
  {
    public const string FileName = "build.spec.json";

    public required string BuildId { get; set; }

    public required string SourceName { get; set; }

    public required string BranchName { get; set; }

    public required string CommitId{ get; set; }

    public required string AuthorName { get; set; }

    public required string AuthorEmail { get; set; }
  }
}
