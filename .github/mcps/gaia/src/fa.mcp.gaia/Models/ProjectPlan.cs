namespace FrostAura.MCP.Gaia.Models;

/// <summary>
/// Represents a project plan for managing Tasks
/// </summary>
public class ProjectPlan
{
    /// <summary>
    /// Unique identifier for the project plan
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Name of the project
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Brief description of the project that an AI can understand
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Concise context that will be needed for when the AI agent later uses the plan to build the solution
    /// </summary>
    public string AiAgentBuildContext { get; set; } = string.Empty;

    /// <summary>
    /// Estimated total hours for completing the entire project plan
    /// </summary>
    public double EstimateHours { get; set; } = 0.0;

    /// <summary>
    /// When this plan was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this plan was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Task items associated with this plan (populated when needed)
    /// </summary>
    public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
