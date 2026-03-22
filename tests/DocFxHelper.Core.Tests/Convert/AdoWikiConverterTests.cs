using DocFxHelper.Core.Convert;

namespace DocFxHelper.Core.Tests.Convert;

public class AdoWikiConverterTests
{
  [Fact]
  public void ConvertMermaidCodeDelimiters_Rewrites_Mermaid_Fences_Only()
  {
    var markdown =
      """
      Before

      ::: mermaid
      graph TD;
          A-->B;
      :::

      ::: note
      keep this block alone
      :::
      """
      .ReplaceLineEndings("\n");

    var updated = AdoWikiConverter.ConvertMermaidCodeDelimiters(markdown);

    var expected =
      """
      Before

      ``` mermaid
      graph TD;
          A-->B;
      ```

      ::: note
      keep this block alone
      :::
      """
      .ReplaceLineEndings("\n");

    Assert.Equal(expected, updated);
  }

  [Fact]
  public void ConvertMermaidCodeDelimiters_Preserves_Indented_Fences()
  {
    var markdown =
      """
        ::: mermaid
        graph TD;
        :::
      """
      .ReplaceLineEndings("\n");

    var updated = AdoWikiConverter.ConvertMermaidCodeDelimiters(markdown);

    var expected =
      """
        ``` mermaid
        graph TD;
        ```
      """
      .ReplaceLineEndings("\n");

    Assert.Equal(expected, updated);
  }
}
