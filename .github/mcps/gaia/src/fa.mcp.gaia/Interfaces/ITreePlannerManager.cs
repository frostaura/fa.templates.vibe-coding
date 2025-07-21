using FrostAura.MCP.Gaia.Models;
using FrostAura.MCP.Gaia.Enums;

namespace FrostAura.MCP.Gaia.Interfaces;

/// <summary>
/// Interface for tree planner management operations
/// </summary>
public interface ITreePlannerManager
{
    /// <summary>
    /// Starts a new project plan (session)
    /// </summary>
    /// <param name="projectName">Name of the project</param>
    /// <param name="description">Brief description of the project</param>
    /// <param name="aiAgentBuildContext">Context for AI agent</param>
    /// <param name="estimateHours">Estimated total hours for the project</param>
    /// <returns>The created plan</returns>
    Task<ProjectPlan> StartNewSessionAsync(string projectName, string description, string aiAgentBuildContext = "", double estimateHours = 0.0);

    /// <summary>
    /// Adds a new TODO item to a session
    /// </summary>
    /// <param name="sessionId">ID of the project session</param>
    /// <param name="title">Title/description of the TODO</param>
    /// <param name="description">Detailed description with acceptance criteria</param>
    /// <param name="tags">Tags for grouping</param>
    /// <param name="groups">Groups for organizing TODOs (e.g., releases, components)</param>
    /// <param name="parentTodoId">ID of parent TODO if this is nested</param>
    /// <param name="estimateHours">Estimated hours for this TODO</param>
    /// <returns>Created TODO item</returns>
    Task<TodoItem> AddTodoAsync(string sessionId, string title, string description = "", List<string>? tags = null, List<string>? groups = null, string? parentTodoId = null, double estimateHours = 0.0);

    /// <summary>
    /// Updates the status of a TODO item
    /// </summary>
    /// <param name="todoId">ID of the TODO item to update</param>
    /// <param name="newStatus">New status for the TODO</param>
    /// <returns>Updated TODO item</returns>
    Task<TodoItem> UpdateTodoStatusAsync(string todoId, TodoStatus newStatus);

    /// <summary>
    /// Gets all project plans (sessions)
    /// </summary>
    /// <returns>List of all plans</returns>
    Task<List<ProjectPlan>> GetAllSessionsAsync();

    /// <summary>
    /// Gets all TODOs for a specific plan (session) with their hierarchical structure
    /// </summary>
    /// <param name="sessionId">ID of the plan to get TODOs for</param>
    /// <returns>The plan with populated TODO hierarchy</returns>
    Task<ProjectPlan> GetTodosBySessionAsync(string sessionId);

    /// <summary>
    /// Gets the overall progress for a specific session
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <returns>Progress information including total todos, completed todos, and percentage</returns>
    Task<SessionProgress> GetSessionProgressAsync(string sessionId);

    /// <summary>
    /// Gets the current in-progress TODO item (first incomplete task) for a session
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <returns>Current in-progress TODO or null if all tasks are completed</returns>
    Task<TodoItem?> GetCurrentInProgressTodoAsync(string sessionId);
}
