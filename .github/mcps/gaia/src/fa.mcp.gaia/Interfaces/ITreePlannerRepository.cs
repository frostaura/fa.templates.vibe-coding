using FrostAura.MCP.Gaia.Models;
using FrostAura.MCP.Gaia.Enums;

namespace FrostAura.MCP.Gaia.Interfaces;

/// <summary>
/// Interface for tree planner repository operations
/// </summary>
public interface ITreePlannerRepository
{
    /// <summary>
    /// Adds a new project plan (session)
    /// </summary>
    /// <param name="session">The plan to add</param>
    /// <returns>The added plan</returns>
    Task<ProjectPlan> AddSessionAsync(ProjectPlan session);

    /// <summary>
    /// Adds a new TODO item to the database
    /// </summary>
    /// <param name="todo">The TODO item to add</param>
    /// <returns>Task representing the operation</returns>
    Task AddTodoAsync(TodoItem todo);

    /// <summary>
    /// Updates the status of a TODO item
    /// </summary>
    /// <param name="todoId">ID of the TODO to update</param>
    /// <param name="newStatus">New status</param>
    /// <returns>Task representing the operation</returns>
    Task UpdateTodoStatusAsync(string todoId, TodoStatus newStatus);

    /// <summary>
    /// Gets a project plan by ID
    /// </summary>
    /// <param name="sessionId">ID of the plan to retrieve</param>
    /// <returns>The plan or null if not found</returns>
    Task<ProjectPlan?> GetSessionByIdAsync(string sessionId);

    /// <summary>
    /// Gets all project plans
    /// </summary>
    /// <returns>List of all plans</returns>
    Task<List<ProjectPlan>> GetAllSessionsAsync();

    /// <summary>
    /// Gets all TODO items for a session
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <returns>List of TODO items for the session</returns>
    Task<List<TodoItem>> GetTodosBySessionAsync(string sessionId);

    /// <summary>
    /// Gets all TODO items
    /// </summary>
    /// <returns>List of all TODO items</returns>
    Task<List<TodoItem>> GetAllTodosAsync();

    /// <summary>
    /// Gets a TODO item by ID
    /// </summary>
    /// <param name="todoId">ID of the TODO item</param>
    /// <returns>TODO item or null if not found</returns>
    Task<TodoItem?> GetTodoByIdAsync(string todoId);

    /// <summary>
    /// Updates a TODO item
    /// </summary>
    /// <param name="todo">TODO item to update</param>
    /// <returns>Updated TODO item</returns>
    Task<TodoItem> UpdateTodoAsync(TodoItem todo);
}
