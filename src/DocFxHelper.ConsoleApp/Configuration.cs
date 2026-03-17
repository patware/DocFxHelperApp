using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.ConsoleApp
{
  public class Configuration
  {
    public const string SectionName = "DocFxHelperConsole";

    public string WorkingDirectory { get; set; } = ".";
  }
}
