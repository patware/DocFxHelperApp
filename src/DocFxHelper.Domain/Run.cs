namespace DocFxHelper.Domain
{
  public class Run
  {
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public required string WorkingDirectory { get; set; }

    public required RunTrigger Trigger { get; set; }

  }

  
}
