using DocFxHelper.Core.Graph;
using DocFxHelper.Core.Specs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Building
{
  public sealed class DefaultDocFxBuildSourceContributor : DocFxBuildSourceContributor<SourceSpec>
  {
    protected override DocFxBuildFragment Create(SiteNode node, SourceSpec sourceSpec)
    {
      var fragment = new DocFxBuildFragment();

      fragment.Content.Add(DocFxBuildEntryFactory.CreateMarkdownContent(node));

      fragment.Resource.Add(
        DocFxBuildEntryFactory.CreateResource(node, "**/*.{png,jpg,jpeg,gif,svg,webp,ico}")
      );

      return fragment;
    }
  }
}
