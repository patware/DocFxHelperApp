using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Infrastructure.Persistence
{
  public class DocFxHelperDbContext : DbContext
  {
    public DbSet<Domain.Run> Runs { get; set; }

    public DocFxHelperDbContext(DbContextOptions<DocFxHelperDbContext> options) : base(options)
    {
      
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Domain.Run>(entity =>
      {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id).ValueGeneratedNever();

      });
    }



  }
}
