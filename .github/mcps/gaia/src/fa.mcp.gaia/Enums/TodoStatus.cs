namespace FrostAura.MCP.Gaia.Enums;

/// <summary>
/// Status of a TODO item
/// </summary>
public enum TodoStatus
{
    /// <summary>
    /// TODO is planned but not started
    /// </summary>
    Todo = 0,

    /// <summary>
    /// TODO is in progress
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// TODO is completed
    /// </summary>
    Completed = 2,

    /// <summary>
    /// TODO is blocked or on hold
    /// </summary>
    Blocked = 3,

    /// <summary>
    /// TODO has been cancelled
    /// </summary>
    Cancelled = 4
}
