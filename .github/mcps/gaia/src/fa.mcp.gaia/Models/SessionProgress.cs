namespace FrostAura.MCP.Gaia.Models;

/// <summary>
/// Represents progress information for a project session
/// </summary>
public class SessionProgress
{
    /// <summary>
    /// ID of the project session
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Name of the project session
    /// </summary>
    public string SessionName { get; set; } = string.Empty;

    /// <summary>
    /// Total number of TODOs in the session
    /// </summary>
    public int TotalTodos { get; set; }

    /// <summary>
    /// Number of completed TODOs
    /// </summary>
    public int CompletedTodos { get; set; }

    /// <summary>
    /// Number of TODOs currently in progress
    /// </summary>
    public int InProgressTodos { get; set; }

    /// <summary>
    /// Number of pending TODOs (not started)
    /// </summary>
    public int PendingTodos { get; set; }

    /// <summary>
    /// Completion percentage (0-100)
    /// </summary>
    public double CompletionPercentage { get; set; }

    /// <summary>
    /// Total estimated hours for the project
    /// </summary>
    public double ProjectEstimateHours { get; set; }

    /// <summary>
    /// Total estimated hours from all TODOs combined
    /// </summary>
    public double TotalTodoEstimateHours { get; set; }

    /// <summary>
    /// Estimated hours from completed TODOs
    /// </summary>
    public double CompletedTodoEstimateHours { get; set; }

    /// <summary>
    /// Estimated hours from TODOs in progress
    /// </summary>
    public double InProgressTodoEstimateHours { get; set; }

    /// <summary>
    /// Estimated hours from pending TODOs
    /// </summary>
    public double PendingTodoEstimateHours { get; set; }

    /// <summary>
    /// Completion percentage based on estimated hours (0-100)
    /// </summary>
    public double EstimateCompletionPercentage { get; set; }
}
