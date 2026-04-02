using DocFxHelper.Core.Properties;
using DocFxHelper.Core.Resources;
using DocFxHelper.Core.Specs;
using DocFxHelper.Infrastructure;
using Markdig.Syntax;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace DocFxHelper.Core.Graph
{
  public class GraphBuilder(
    ILogger<GraphBuilder> logger, 
    IFileSystem fileSystem,
    ISpecLoader specLoader) : IGraphBuilder
  {
    private readonly ILogger<GraphBuilder> _logger = logger;
    private readonly IFileSystem _fileSystem = fileSystem;
    private readonly ISpecLoader _specLoader = specLoader;

    public async Task<SiteGraph> BuildAsync(string path, CancellationToken ct = default!)
    {
      _logger.LogInformation("Building Graph from {path}", path);

      var buildContext = await GetBuildContextAsync(path);

      if (buildContext == null)
      {
        return SiteGraph.Default;
      }

      var siteGraph = await GetGraphFromBuildContextAsync(buildContext);

      return siteGraph;
    }

    /// <summary>
    /// The key is the Id of the SourceNode.Id
    /// </summary>
    /// <param name="buildContext"></param>
    /// <returns></returns>
    private static async Task<SiteGraph> GetGraphFromBuildContextAsync(GraphBuildContext buildContext)
    {

      var siteGraph = new SiteGraph
      {
        BuildContext = buildContext
      };

      if (buildContext.Master.Root == null)
        return siteGraph;

      var queue = new Queue<NodeItem>();

      queue.Enqueue(buildContext.Master.Root);

      var parentDic = new Dictionary<string, NodeItem>();

      do
      {

        var nodeItem = queue.Dequeue();

        var sourceSpec = buildContext.Sources[nodeItem.ResourceId];
        var buildSpec = buildContext.Builds[nodeItem.ResourceId];

        foreach(var child in nodeItem.Children)
        {
          parentDic.Add(child.ResourceId, nodeItem);
          queue.Enqueue(child);
        }

        string dest = string.Empty;

        NodeItem? parentNodeItem = null;

        if (parentDic.ContainsKey(nodeItem.ResourceId))
        {
          parentNodeItem = parentDic[nodeItem.ResourceId];

          var parentSiteNode = siteGraph.Items[parentNodeItem.ResourceId];

          var segments = new List<string>();

          segments.AddRange(parentSiteNode
            .Item
            .Dest
            .Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries));

          segments.AddRange(nodeItem
            .TargetRelativePath
            .Replace('/', Path.DirectorySeparatorChar)
            .Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries));

          dest = string.Join(Path.DirectorySeparatorChar, segments);
           
        }



        var siteNode = new SiteNode
        {
          Id = nodeItem.ResourceId,
          DisplayName = sourceSpec.DisplayName,
          SourceSpec = sourceSpec,
          BuildSpec = buildSpec,
          Path = nodeItem.TargetRelativePath,
          Dest = dest,
          ShowInToc = nodeItem.ShowInToc,
          DefaultPage = sourceSpec.DefaultPage,
          TocItemInsertAtIndex = nodeItem.TocItemInsertAtIndex,
          ParentTocDisplayName = nodeItem.ParentTocDisplayName
        };
        
        if (parentNodeItem == null)
        {
          siteGraph.Add(siteNode.Id, siteNode);
        }
        else
        {
          siteGraph.Add(parentNodeItem.ResourceId, nodeItem.ResourceId, siteNode);          
        }

      }while(queue.Count > 0);


      return siteGraph;
    }
    
    private async Task<GraphBuildContext> GetBuildContextAsync(string path)
    {
      _logger.LogInformation("Fetching list of sub folders");
      var sourceSubFolders = _fileSystem.GetDirectories(path, false);

      (string? masterDirectory, MasterSpec? master) = await GetMasterSpecFromSubFoldersAsync(sourceSubFolders);

      if (masterDirectory == null || master == null)
      {
        return GraphBuildContext.Default;
      }

      IReadOnlyDictionary<string,SourceSpec> sourcesDic = await GetSourceSpecsFromSubfoldersAsync(sourceSubFolders, master);
      IReadOnlyDictionary<string, BuildSpec> buildDic = await GetBuildSpecsFromSubfoldersAsync(sourceSubFolders, master);

      var gbc = new GraphBuildContext
      {
        Master = master,
        Sources = sourcesDic,
        Builds = buildDic
      };


      return gbc;

    }

    private async Task<(string?, MasterSpec?)> GetMasterSpecFromSubFoldersAsync(IReadOnlyList<string> sourceSubFolders)
    {
      _logger.LogInformation("Getting Master Spec from sources folders");
      var masterDirectory = sourceSubFolders.FirstOrDefault(s => (new System.IO.DirectoryInfo(s)).Name == "_master");

      if (masterDirectory == null)
      {
        _logger.LogInformation("No _master folder found in the sources directory");
        return (null, null);
      }

      var master = await _specLoader.LoadMasterSpecAsync(System.IO.Path.Combine(masterDirectory, Specs.MasterSpec.FileName));

      return (masterDirectory, master);
    }

    private async Task<IReadOnlyDictionary<string,SourceSpec>> GetSourceSpecsFromSubfoldersAsync(IEnumerable<string> sourceSubFolders, MasterSpec master)
    {

      var dic = new Dictionary<string,SourceSpec>();

      if (master.Root == null)
      {
        return dic;
      }

      var queue = new System.Collections.Generic.Queue<NodeItem>();

      queue.Enqueue(master.Root);

      while (queue.Count > 0)
      {
        var ni = queue.Dequeue();

        foreach(var child in ni.Children)
        {
          queue.Enqueue(child);
        }

        var rsxFolder = sourceSubFolders.FirstOrDefault(s => (new System.IO.DirectoryInfo(s)).Name == ni.ResourceId);

        if (rsxFolder != null && _fileSystem.DirectoryExists(rsxFolder))
        {
          var sourceSpec = await _specLoader.LoadSourceSpecAsync(System.IO.Path.Combine(rsxFolder, Specs.SourceSpec.FileName));

          if (sourceSpec != null)
          {
            dic.Add(ni.ResourceId, sourceSpec);
          }

        }
      }

      return dic;

    }

    private async Task<IReadOnlyDictionary<string, BuildSpec>> GetBuildSpecsFromSubfoldersAsync(IEnumerable<string> sourceSubFolders, MasterSpec master)
    {

      var dic = new Dictionary<string, BuildSpec>();

      if (master.Root == null)
      {
        return dic;
      }

      var queue = new System.Collections.Generic.Queue<NodeItem>();

      queue.Enqueue(master.Root);

      while (queue.Count > 0)
      {
        var ni = queue.Dequeue();

        foreach (var child in ni.Children)
        {
          queue.Enqueue(child);
        }

        var rsxFolder = sourceSubFolders.FirstOrDefault(s => (new System.IO.DirectoryInfo(s)).Name == ni.ResourceId);

        if (rsxFolder != null && _fileSystem.DirectoryExists(rsxFolder))
        {
          var buildSpec = await _specLoader.LoadBuildSpecAsync(System.IO.Path.Combine(rsxFolder, Specs.BuildSpec.FileName));

          if (buildSpec != null)
          {
            dic.Add(ni.ResourceId, buildSpec);
          }

        }
      }

      return dic;

    }
  }
}
