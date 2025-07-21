using FrostAura.MCP.Gaia.Interfaces;
using FrostAura.MCP.Gaia.Models;
using FrostAura.MCP.Gaia.Enums;
using Microsoft.Extensions.Logging;

namespace FrostAura.MCP.Gaia.Data;

/// <summary>
/// Repository for tree planner operations with proper error handling
/// </summary>
public class TreePlannerRepository : ITreePlannerRepository
{
    private readonly TreePlannerDbContext _dbContext;
    private readonly ILogger<TreePlannerRepository> _logger;

    public TreePlannerRepository(TreePlannerDbContext dbContext, ILogger<TreePlannerRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Adds a new project plan (session)
    /// </summary>
    /// <param name="session">The plan to add</param>
    /// <returns>The added plan</returns>
    public async Task<ProjectPlan> AddSessionAsync(ProjectPlan session)
    {
        _logger.LogInformation("Adding new project plan: {SessionId}", session.Id);
        
        try
        {
            await _dbContext.AddSessionAsync(session);
            return session;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add project plan: {SessionId}", session.Id);
            throw new InvalidOperationException($"Failed to add project plan: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Adds a new TODO item to the database
    /// </summary>
    /// <param name="todo">TODO item to add</param>
    public async Task AddTodoAsync(TodoItem todo)
    {
        _logger.LogInformation("Adding new TODO item: {TodoId} for session: {SessionId}", todo.Id, todo.SessionId);
        
        try
        {
            await _dbContext.AddTodoAsync(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add TODO item: {TodoId}", todo.Id);
            throw new InvalidOperationException($"Failed to add TODO item: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Updates the status of a TODO item
    /// </summary>
    /// <param name="todoId">ID of the TODO to update</param>
    /// <param name="newStatus">New status</param>
    public async Task UpdateTodoStatusAsync(string todoId, TodoStatus newStatus)
    {
        _logger.LogInformation("Updating TODO status: {TodoId} to {Status}", todoId, newStatus);
        
        try
        {
            await _dbContext.UpdateTodoStatusAsync(todoId, newStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update TODO status: {TodoId}", todoId);
            throw new InvalidOperationException($"Failed to update TODO status: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets a project plan by ID
    /// </summary>
    /// <param name="sessionId">ID of the plan to retrieve</param>
    /// <returns>The plan or null if not found</returns>
    public async Task<ProjectPlan?> GetSessionByIdAsync(string sessionId)
    {
        _logger.LogInformation("Getting project plan by ID: {SessionId}", sessionId);
        
        try
        {
            return await _dbContext.GetSessionByIdAsync(sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get project plan by ID: {SessionId}", sessionId);
            throw new InvalidOperationException($"Failed to get project plan: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets all project plans
    /// </summary>
    /// <returns>List of all plans</returns>
    public async Task<List<ProjectPlan>> GetAllSessionsAsync()
    {
        _logger.LogInformation("Getting all project plans");
        
        try
        {
            return await _dbContext.GetAllSessionsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all project plans");
            throw new InvalidOperationException($"Failed to get all project plans: {ex.Message}", ex);
        }
    }

    // Stub implementations for missing interface members
    public async Task<List<TodoItem>> GetTodosBySessionAsync(string sessionId)
    {
        _logger.LogInformation("Getting all TODOs for session: {SessionId}", sessionId);
        
        try
        {
            var session = await _dbContext.GetSessionByIdAsync(sessionId);
            if (session?.Todos == null)
            {
                _logger.LogWarning("Session {SessionId} not found or has no TODOs", sessionId);
                return new List<TodoItem>();
            }
            
            // Flatten the hierarchical structure to return all TODOs in the session
            return FlattenTodosRecursively(session.Todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get TODOs for session: {SessionId}", sessionId);
            throw new InvalidOperationException($"Failed to get TODOs for session: {ex.Message}", ex);
        }
    }

    public async Task<List<TodoItem>> GetAllTodosAsync()
    {
        _logger.LogInformation("Getting all TODOs from all sessions");
        
        try
        {
            var sessions = await _dbContext.GetAllSessionsAsync();
            var allTodos = new List<TodoItem>();
            
            foreach (var session in sessions)
            {
                if (session.Todos != null)
                {
                    allTodos.AddRange(FlattenTodosRecursively(session.Todos));
                }
            }
            
            return allTodos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all TODOs");
            throw new InvalidOperationException($"Failed to get all TODOs: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Flattens a hierarchical TODO structure into a flat list
    /// </summary>
    /// <param name="todos">Hierarchical TODOs</param>
    /// <returns>Flat list of all TODOs</returns>
    private List<TodoItem> FlattenTodosRecursively(List<TodoItem> todos)
    {
        var result = new List<TodoItem>();
        
        foreach (var todo in todos)
        {
            result.Add(todo);
            
            if (todo.Children != null && todo.Children.Any())
            {
                result.AddRange(FlattenTodosRecursively(todo.Children));
            }
        }
        
        return result;
    }

    public async Task<TodoItem?> GetTodoByIdAsync(string todoId)
    {
        _logger.LogInformation("Getting TODO by ID: {TodoId}", todoId);
        
        try
        {
            var sessions = await _dbContext.GetAllSessionsAsync();
            
            // Search recursively through all sessions and their hierarchical todos
            foreach (var session in sessions)
            {
                var todo = FindTodoRecursively(session.Todos, todoId);
                if (todo != null)
                {
                    _logger.LogInformation("Found TODO {TodoId} in session {SessionId}", todoId, session.Id);
                    return todo;
                }
            }
            
            _logger.LogWarning("TODO {TodoId} not found in any session", todoId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get TODO by ID: {TodoId}", todoId);
            throw new InvalidOperationException($"Failed to get TODO by ID: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Recursively searches for a TODO by ID in a hierarchical list
    /// </summary>
    /// <param name="todos">List of TODOs to search</param>
    /// <param name="todoId">ID to find</param>
    /// <returns>Found TODO or null</returns>
    private TodoItem? FindTodoRecursively(List<TodoItem> todos, string todoId)
    {
        foreach (var todo in todos)
        {
            if (todo.Id == todoId)
            {
                return todo;
            }
            
            // Search in children recursively
            if (todo.Children != null)
            {
                var childResult = FindTodoRecursively(todo.Children, todoId);
                if (childResult != null)
                {
                    return childResult;
                }
            }
        }
        return null;
    }

    public async Task<TodoItem> UpdateTodoAsync(TodoItem todo)
    {
        // TODO: Implement actual TODO update in DbContext
        // For now, this is a stub to allow compilation
        await Task.CompletedTask;
        return todo;
    }
}
