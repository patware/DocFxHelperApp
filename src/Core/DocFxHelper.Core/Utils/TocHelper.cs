using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace DocFxHelper.Core.Utils
{
  public class TocHelper : ITocHelper
  {
   

    public Graph.Toc GetToc(string yaml)
    {
      var deserializer = new DeserializerBuilder()
        .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

      var toc = deserializer.Deserialize<Graph.Toc>(yaml);

      return toc;
    }

    public string GetString(Graph.Toc toc)
    {
      var serializer = new SerializerBuilder()
        .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitEmptyCollections)
        .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
        .Build();

      var s = serializer.Serialize(toc);

      return s;

    }
  }
}
