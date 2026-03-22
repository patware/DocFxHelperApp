using DocFxHelper.Core.Resources;
using DocFxHelper.Core.Specs;
using DocFxHelper.Infrastructure;
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

      var (rootNode, nodes) = await GetNodesFromBuildContextAsync(buildContext);

      if (rootNode == null)
      {
        return SiteGraph.Default;
      }

      var siteGraph = new SiteGraph
      {
        // Provide a placeholder root. Replace with a real SiteNode when available.
        Root = rootNode,

        // Create an empty dictionary for Sources. Populate this with real SourceNode instances when mapping is implemented.
        Sources = nodes,

        // Provide a placeholder build context. Replace with a real GraphBuildContext when available.
        BuildContext = buildContext
      };


      return siteGraph;
    }

    /// <summary>
    /// The key is the Id of the SourceNode.Id
    /// </summary>
    /// <param name="buildContext"></param>
    /// <returns></returns>
    private static async Task<(SiteNode?, IReadOnlyDictionary<string, SiteNode>)> GetNodesFromBuildContextAsync(GraphBuildContext buildContext)
    {

      var dic = new Dictionary<string, SiteNode>();

      if (buildContext.Master.Root == null)
        return (null, dic);

      var stack = GetNodeItemStack(buildContext);
            
      SiteNode? rootNode = null;

      while (stack.Count > 0)
      {
        var nodeItem = stack.Pop();
        
        var sourceSpec = buildContext.Sources[nodeItem.ResourceId];

        var childList = new List<SiteNode>();

        foreach(var childId in nodeItem.Children.Select(s => s.ResourceId))
        {
          childList.Add(dic[childId]);
        }

        var sn = new SiteNode { 
          Id = nodeItem.ResourceId,
          DisplayName = sourceSpec.DisplayName,
          SourceSpec = sourceSpec,
          Path = nodeItem.TargetRelativePath,
          ShowInToc = nodeItem.ShowInToc,
          DefaultPage = sourceSpec.DefaultPage,
          TocItemInsertAtIndex = nodeItem.TocItemInsertAtIndex,
          ParentTocDisplayName = nodeItem.ParentTocDisplayName,
          Children = childList
        };
        
        if (sn.Id == buildContext.Master.Root.ResourceId)
        {
          rootNode = sn;
        }

        dic.Add(sn.Id, sn);

      }

      return (rootNode, dic);
    }

    private static System.Collections.Generic.Stack<NodeItem> GetNodeItemStack(GraphBuildContext buildContext)
    {
      var stack = new System.Collections.Generic.Stack<NodeItem>();
      var queue = new System.Collections.Generic.Queue<NodeItem>();

      if (buildContext.Master == null || buildContext.Master.Root == null)
      {
        return stack;
      }

      queue.Enqueue(buildContext.Master.Root);

      while (queue.Count > 0)
      {
        var ni = queue.Dequeue();
        stack.Push(ni);

        foreach (var child in ni.Children)
        {
          queue.Enqueue(child);
        }
      }

      return stack;
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

      var gbc = new GraphBuildContext
      {
        Master = master,
        Sources = sourcesDic
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
  }
}
