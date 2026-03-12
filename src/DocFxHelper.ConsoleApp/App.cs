using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.ConsoleApp
{
  public class App(DocFxHelper.Core.Engine.IProcessor processor )
  {
    private readonly DocFxHelper.Core.Engine.IProcessor _processor = processor;


    public async Task RunAsync()
    {
      var run = new DocFxHelper.Domain.Run
      {
        Id = Guid.NewGuid(),
        Name = "Manual Run",
        WorkingDirectory = Environment.CurrentDirectory,
        Trigger = new DocFxHelper.Domain.RunTrigger
        {
          Source = "DocFxHelper Console App",
          Author = Environment.UserName
        }
      };

      await _processor.ProcessAsync(run);
    }
  }
}
