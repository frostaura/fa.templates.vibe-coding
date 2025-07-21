using FrostAura.MCP.Gaia.Enums;

namespace FrostAura.MCP.Gaia.Models;

/// <summary>
/// Represents a TODO item with support for nested TODOs
/// </summary>
public class TodoItem
{
    /// <summary>
    /// Unique identifier for the TODO item
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// ID of the project plan this TODO belongs to (kept as SessionId for API compatibility)
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// ID of the parent TODO if this is a nested TODO
    /// </summary>
    public string? ParentTodoId { get; set; }

    /// <summary>
    /// Title/description of the TODO that an AI can understand
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description with acceptance criteria
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Status of the TODO
    /// </summary>
    public TodoStatus Status { get; set; } = TodoStatus.Todo;

    /// <summary>
    /// Tags for grouping and categorization
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// Collection of group names this TODO belongs to (e.g., "Release 1.0", "Backend", "Frontend")
    /// </summary>
    public List<string> Groups { get; set; } = new List<string>();

    /// <summary>
    /// Estimated hours for completing this TODO item
    /// </summary>
    public double EstimateHours { get; set; } = 0.0;

    /// <summary>
    /// When this TODO was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this TODO was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this TODO was completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Child TODO items nested under this TODO
    /// </summary>
    public List<TodoItem> Children { get; set; } = new List<TodoItem>();
}
