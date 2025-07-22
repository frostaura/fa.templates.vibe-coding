namespace FrostAura.MCP.Gaia.Interfaces;

/// <summary>
/// Interface for task planner management operations - MCP server tools
/// </summary>
public interface ITaskPlannerManager
{
    /// <summary>
    /// Creates a new project plan via MCP
    /// </summary>
    /// <param name="projectName">Name of the project</param>
    /// <param name="description">Brief description that an AI can understand</param>
    /// <param name="aiAgentBuildContext">Concise context that will be needed for when the AI agent later uses the plan to build the solution</param>
    /// <param name="estimateHours">Estimated total hours for completing the project</param>
    /// <returns>JSON string containing the created project plan</returns>
    Task<string> NewPlanAsync(string projectName, string description, string aiAgentBuildContext, double estimateHours);

    /// <summary>
    /// Lists all project plans via MCP
    /// </summary>
    /// <param name="hideCompleted">Optional string to hide completed plans ("true" to hide, anything else to show all)</param>
    /// <returns>JSON string containing all project plans with their status and progress information</returns>
    Task<string> ListPlansAsync(string? hideCompleted = null);

    /// <summary>
    /// Gets all tasks from a specific plan via MCP
    /// </summary>
    /// <param name="planId">ID of the plan to get tasks from</param>
    /// <param name="hideCompleted">Optional string to hide completed tasks ("true" to hide, anything else to show all)</param>
    /// <returns>JSON string containing all tasks for the specified plan</returns>
    Task<string> GetTasksFromPlan(string planId, string? hideCompleted = null);

    /// <summary>
    /// Adds a new Task item via MCP
    /// </summary>
    /// <param name="planId">ID of the project plan</param>
    /// <param name="title">Title/description of the Task</param>
    /// <param name="description">Detailed description with acceptance criteria</param>
    /// <param name="tags">Comma-separated tags for grouping</param>
    /// <param name="groups">Comma-separated groups for organizing Tasks (e.g., releases, components)</param>
    /// <param name="parentTaskId">ID of parent Task if this is nested</param>
    /// <param name="estimateHours">Estimated hours for completing this Task</param>
    /// <returns>JSON string containing the created Task item</returns>
    Task<string> AddTaskToPlanAsync(string planId, string title, string description, string tags, string groups, string? parentTaskId, double estimateHours);

    /// <summary>
    /// Complete the next outstanding leaf node task and return the context of the next outstanding task via MCP
    /// </summary>
    /// <param name="planId">ID of the plan</param>
    /// <returns>JSON string containing the completed task and next task context</returns>
    Task<string> NextTaskFromPlanAsync(string planId);

    /// <summary>
    /// Gets a task with all its children in a hierarchical structure
    /// </summary>
    /// <param name="taskId">ID of the task to retrieve</param>
    /// <returns>JSON string containing the task with its nested children</returns>
    Task<string> GetTaskWithChildrenByIdAsync(string taskId);
}
