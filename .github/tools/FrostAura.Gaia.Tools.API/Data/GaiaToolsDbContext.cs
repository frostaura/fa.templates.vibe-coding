using FrostAura.Gaia.Tools.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FrostAura.Gaia.Tools.API.Data;

/// <summary>
/// Entity Framework DbContext for the Gaia Tools API
/// </summary>
public class GaiaToolsDbContext : DbContext
{
    public GaiaToolsDbContext(DbContextOptions<GaiaToolsDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Project plans
    /// </summary>
    public DbSet<ProjectPlan> ProjectPlans { get; set; }

    /// <summary>
    /// Task items
    /// </summary>
    public DbSet<TaskItem> TaskItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ProjectPlan entity
        modelBuilder.Entity<ProjectPlan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.AiAgentBuildContext).HasMaxLength(2000);
            entity.Property(e => e.CreatorIdentity).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // Configure the relationship with TaskItems
            entity.HasMany(p => p.Tasks)
                  .WithOne()
                  .HasForeignKey(t => t.PlanId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure TaskItem entity
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.PlanId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ParentTaskId).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.AcceptanceCriteria).HasMaxLength(2000);
            entity.Property(e => e.EstimateHours).HasPrecision(10, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // Configure Tags as JSON
            entity.Property(e => e.Tags)
                  .HasConversion(
                      v => string.Join(',', v),
                      v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                  .HasMaxLength(1000);

            // Configure Groups as JSON
            entity.Property(e => e.Groups)
                  .HasConversion(
                      v => string.Join(',', v),
                      v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                  .HasMaxLength(1000);

            // Configure self-referencing relationship for parent-child tasks
            entity.HasMany(t => t.Children)
                  .WithOne()
                  .HasForeignKey(t => t.ParentTaskId)
                  .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes that could cause cycles

            // Index for performance
            entity.HasIndex(e => e.PlanId);
            entity.HasIndex(e => e.ParentTaskId);
            entity.HasIndex(e => e.Status);
        });
    }
}
