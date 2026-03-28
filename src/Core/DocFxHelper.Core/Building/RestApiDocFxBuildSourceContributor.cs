using DocFxHelper.Core.Graph;
using DocFxHelper.Core.Specs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;

namespace DocFxHelper.Core.Building
{
  public sealed class RestApiDocFxBuildSourceContributor : DocFxBuildSourceContributor<RestApiSourceSpec>
  {
    protected override DocFxBuildFragment Create(SiteNode node, RestApiSourceSpec sourceSpec)
    {
      var fragment = new DocFxBuildFragment();

      var content = DocFxBuildEntryFactory.CreateBaseContent(node);
      content["files"] = new JsonArray(sourceSpec.ApiJson);
      fragment.Content.Add(content);

      fragment.Resource.Add(
        DocFxBuildEntryFactory.CreateResource(
          node,
          "**/*.{png,jpg,jpeg,gif,svg,webp,ico}"
        ));


      return fragment;
    }
  }
}
