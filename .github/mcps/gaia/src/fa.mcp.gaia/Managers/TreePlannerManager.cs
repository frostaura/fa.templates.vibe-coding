using FrostAura.MCP.Gaia.Interfaces;
using FrostAura.MCP.Gaia.Models;
using FrostAura.MCP.Gaia.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace FrostAura.MCP.Gaia.Managers;

/// <summary>
/// Manager for TODO planning operations with integrated MCP tools
/// </summary>
[McpServerToolType]
public class TreePlannerManager : ITreePlannerManager
{
    private readonly ITreePlannerRepository _repository;
    private readonly ILogger<TreePlannerManager> _logger;
    private readonly bool _throwExceptionsInsteadOfReturningErrors;

    /// <summary>
    /// Initializes a new instance of the TreePlannerManager
    /// </summary>
    /// <param name="repository">TODO repository</param>
    /// <param name="logger">Logger instance</param>
    /// <param name="configuration">Configuration to check for error handling preferences</param>
    public TreePlannerManager(ITreePlannerRepository repository, ILogger<TreePlannerManager> logger, IConfiguration configuration)
    {
        _repository = repository;
        _logger = logger;
        
        // Check if we should throw exceptions instead of returning error JSON
        // This helps with debugging and testing scenarios
        _throwExceptionsInsteadOfReturningErrors = configuration.GetValue<bool>("TreePlanner:ThrowExceptionsInsteadOfErrorResponses", false);
        
        if (_throwExceptionsInsteadOfReturningErrors)
        {
            _logger.LogInformation("🚨 [CONFIG] TreePlannerManager configured to THROW exceptions instead of returning error responses");
        }
    }

    /// <summary>
    /// Starts a new project session via MCP
    /// </summary>
    /// <param name="projectName">Name of the project</param>
    /// <param name="description">Brief description that an AI can understand</param>
    /// <param name="aiAgentBuildContext">Concise context that will be needed for when the AI agent later uses the plan to build the solution</param>
    /// <param name="estimateHours">Estimated total hours for completing the project</param>
    /// <returns>JSON string containing the created project session</returns>
    [McpServerTool]
    [Description("Starts a new project session for managing TODOs. Ideal for tracking tasks and features of complex projects and plans. The response is your TODO session id, which you must use to manage your TODOs.")]
    public async Task<string> GaiaTreePlannerStartSessionAsync(
        [Description("Name of the project")] string projectName,
        [Description("Brief description of the project")] string description,
        [Description("Concise context that will be needed for when the AI agent later uses the plan to build the solution")] string aiAgentBuildContext,
        [Description("Estimated total hours for completing the project")] double estimateHours = 0.0)
    {
        _logger.LogInformation("🚀 [START SESSION] Initializing new project session for '{ProjectName}'", projectName);
        _logger.LogInformation("📝 Project Description: {Description}", description);
        _logger.LogInformation("🤖 AI Build Context: {AiAgentBuildContext}", aiAgentBuildContext);
        _logger.LogInformation("⏱️ Estimated Hours: {EstimateHours}", estimateHours);

        try
        {
            var session = await ((ITreePlannerManager)this).StartNewSessionAsync(projectName, description, aiAgentBuildContext, estimateHours);
            var json = JsonSerializer.Serialize(session);
            _logger.LogInformation("✅ [START SESSION] Successfully created project session '{ProjectName}' with ID: {SessionId}", 
                projectName, session.Id);
            return json;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [START SESSION] Failed to create project session '{ProjectName}': {ErrorMessage}", 
                projectName, ex.Message);
            
            var detailedError = $"Failed to create project session '{projectName}': {ex.Message}";
            var errorResult = new { 
                error = detailedError, 
                projectName, 
                description, 
                aiAgentBuildContext, 
                estimateHours,
                exceptionType = ex.GetType().Name,
                stackTrace = ex.StackTrace
            };
            return JsonSerializer.Serialize(errorResult);
        }
    }

    /// <summary>
    /// Adds a new TODO item via MCP
    /// </summary>
    /// <param name="sessionId">ID of the project session</param>
    /// <param name="title">Title/description of the TODO</param>
    /// <param name="description">Detailed description with acceptance criteria</param>
    /// <param name="tags">Comma-separated tags for grouping</param>
    /// <param name="groups">Comma-separated groups for organizing TODOs (e.g., releases, components)</param>
    /// <param name="parentTodoId">ID of parent TODO if this is nested</param>
    /// <param name="estimateHours">Estimated hours for completing this TODO</param>
    /// <returns>JSON string containing the created TODO item</returns>
    [McpServerTool]
    [Description("Adds a new TODO item to a project session / plan. 3-levels deep nesting of todos to compartmentalize complex tasksis recommended for plans.")]
    public async Task<string> GaiaTreePlannerAddTodoAsync(
        [Description("ID of the project session / plan, as from the start session reponse.")] string sessionId,
        [Description("Title/description of the TODO that an AI can understand")] string title,
        [Description("Detailed description with acceptance criteria, important references like docs, rules, restrictions, file & directory paths")] string description = "",
        [Description("Comma-separated tags for categorizing TODOs. Like dev, test, analysis etc")] string tags = "",
        [Description("Comma-separated groups for organizing TODOs (e.g., releases, components)")] string groups = "",
        [Description("ID of parent TODO if this is a child of another TODO")] string? parentTodoId = null,
        [Description("Estimated hours for completing this TODO")] double estimateHours = 0.0)
    {
        var parentInfo = !string.IsNullOrEmpty(parentTodoId) ? $" (child of {parentTodoId})" : " (root-level)";
        _logger.LogInformation("📝 [ADD TODO] Creating new TODO '{Title}' in session {SessionId}{ParentInfo}", 
            title, sessionId, parentInfo);
        
        if (!string.IsNullOrWhiteSpace(tags))
            _logger.LogInformation("🏷️  Tags: {Tags}", tags);
        if (!string.IsNullOrWhiteSpace(groups))
            _logger.LogInformation("📂 Groups: {Groups}", groups);
        if (!string.IsNullOrWhiteSpace(description))
            _logger.LogInformation("📋 Description: {Description}", description);
        if (estimateHours > 0)
            _logger.LogInformation("⏱️ Estimated Hours: {EstimateHours}", estimateHours);

        try
        {
            var tagList = string.IsNullOrWhiteSpace(tags) ? new List<string>() : tags.Split(',').Select(t => t.Trim()).ToList();
            var groupList = string.IsNullOrWhiteSpace(groups) ? new List<string>() : groups.Split(',').Select(g => g.Trim()).ToList();
            
            var todo = await ((ITreePlannerManager)this).AddTodoAsync(sessionId, title, description, tagList, groupList, parentTodoId, estimateHours);
            var json = JsonSerializer.Serialize(todo);
            _logger.LogInformation("✅ [ADD TODO] Successfully created TODO '{Title}' with ID: {TodoId}", 
                title, todo.Id);
            return json;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [ADD TODO] Failed to create TODO '{Title}' in session {SessionId}: {ErrorMessage}", 
                title, sessionId, ex.Message);
            
            string detailedError;
            if (ex is ArgumentException)
            {
                detailedError = $"Invalid session ID '{sessionId}' - session not found. Please verify the session ID is correct.";
            }
            else
            {
                detailedError = $"Failed to create TODO '{title}' in session '{sessionId}': {ex.Message}";
            }
            
            var errorResult = new { 
                error = detailedError, 
                sessionId, 
                title,
                exceptionType = ex.GetType().Name,
                stackTrace = ex.StackTrace
            };
            return JsonSerializer.Serialize(errorResult);
        }
    }

    /// <summary>
    /// Updates the status of a TODO item via MCP
    /// </summary>
    /// <param name="todoId">ID of the TODO item to update</param>
    /// <param name="newStatus">New status for the TODO (0=Todo, 1=InProgress, 2=Done)</param>
    /// <returns>JSON string containing the updated TODO item</returns>
    [McpServerTool]
    [Description("Updates the status of a TODO item.")]
    public async Task<string> GaiaTreePlannerUpdateTodoStatusAsync(
        [Description("ID of the TODO item to update")] string todoId,
        [Description("New status for the TODO (0=Todo, 1=InProgress, 2=Done)")] int newStatus)
    {
        var statusNames = new[] { "📋 Todo", "⏳ In Progress", "✅ Done" };
        var statusName = newStatus >= 0 && newStatus < statusNames.Length ? statusNames[newStatus] : $"Unknown({newStatus})";
        
        _logger.LogInformation("🔄 [UPDATE STATUS] Changing TODO {TodoId} status to {StatusName}", todoId, statusName);

        try
        {
            var status = (TodoStatus)newStatus;
            
            // First, update the status using the repository
            await _repository.UpdateTodoStatusAsync(todoId, status);
            
            // Get the updated TODO to return and verify it exists
            var updatedTodo = await _repository.GetTodoByIdAsync(todoId);
            
            if (updatedTodo == null)
            {
                var errorMessage = $"TODO with ID '{todoId}' was not found after status update. This indicates a critical data consistency issue - the TODO may have been deleted or the database may be corrupted.";
                _logger.LogError("💀 [UPDATE STATUS] Critical error: {ErrorMessage}", errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            
            var json = JsonSerializer.Serialize(updatedTodo);
            _logger.LogInformation("✅ [UPDATE STATUS] Successfully updated TODO '{Title}' ({TodoId}) to {StatusName}", 
                updatedTodo.Title, updatedTodo.Id, statusName);
            return json;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [UPDATE STATUS] Failed to update TODO {TodoId} to {StatusName}: {ErrorMessage}", 
                todoId, statusName, ex.Message);
            
            // Check if we should throw exceptions instead of returning error responses
            if (_throwExceptionsInsteadOfReturningErrors)
            {
                throw;
            }
            
            // Provide detailed error context based on exception type
            string detailedError;
            if (ex is ArgumentException)
            {
                detailedError = $"TODO with ID '{todoId}' was not found in the database. Please verify the TODO ID is correct and the TODO hasn't been deleted.";
            }
            else if (ex is InvalidOperationException)
            {
                detailedError = $"Failed to update TODO '{todoId}' status to {statusName}: {ex.Message}";
            }
            else
            {
                detailedError = $"Unexpected error while updating TODO '{todoId}' to status {statusName}: {ex.Message}";
            }
            
            var errorResult = new { 
                error = detailedError, 
                todoId, 
                newStatus,
                statusName,
                exceptionType = ex.GetType().Name,
                stackTrace = ex.StackTrace,
                innerException = ex.InnerException?.Message
            };
            return JsonSerializer.Serialize(errorResult);
        }
    }

    /// <summary>
    /// Gets all project sessions via MCP
    /// </summary>
    /// <returns>JSON string containing all project sessions</returns>
    [McpServerTool]
    [Description("Gets all project sessions / plans with their IDs and details.")]
    public async Task<string> GaiaTreePlannerGetAllSessionsAsync()
    {
        _logger.LogInformation("📋 [GET SESSIONS] Retrieving all project sessions...");

        try
        {
            var sessions = await ((ITreePlannerManager)this).GetAllSessionsAsync();
            var json = JsonSerializer.Serialize(sessions);
            _logger.LogInformation("✅ [GET SESSIONS] Successfully retrieved {Count} project session(s): {SessionNames}", 
                sessions.Count, string.Join(", ", sessions.Select(s => $"'{s.Name}'")));
            return json;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [GET SESSIONS] Failed to retrieve project sessions: {ErrorMessage}", ex.Message);
            var detailedError = $"Failed to retrieve project sessions: {ex.Message}";
            var errorResult = new { 
                error = detailedError,
                exceptionType = ex.GetType().Name,
                stackTrace = ex.StackTrace
            };
            return JsonSerializer.Serialize(errorResult);
        }
    }

    /// <summary>
    /// Gets all TODO items for a specific session via MCP
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <returns>JSON string containing the session with all TODO items</returns>
    [McpServerTool]
    [Description("Gets all TODO items for a specific session / plan with their IDs and details. This is fantastic for retrieving a plan too.")]
    public async Task<string> GaiaTreePlannerGetTodosBySessionAsync(
        [Description("ID of the session to get TODOs for, as from the start session reponse.")] string sessionId)
    {
        _logger.LogInformation("📋 [GET TODOS] Retrieving all TODOs for session {SessionId}...", sessionId);

        try
        {
            var sessionWithTodos = await ((ITreePlannerManager)this).GetTodosBySessionAsync(sessionId);
            var json = JsonSerializer.Serialize(sessionWithTodos);
            
            var statusCounts = sessionWithTodos.Todos.GroupBy(t => t.Status)
                .ToDictionary(g => g.Key, g => g.Count());
            
            _logger.LogInformation("✅ [GET TODOS] Retrieved {Count} TODO(s) for session '{SessionName}': " +
                                 "📋 {TodoCount} pending, ⏳ {InProgressCount} in progress, ✅ {DoneCount} completed", 
                sessionWithTodos.Todos.Count, sessionWithTodos.Name,
                statusCounts.GetValueOrDefault(TodoStatus.Todo, 0),
                statusCounts.GetValueOrDefault(TodoStatus.InProgress, 0),
                statusCounts.GetValueOrDefault(TodoStatus.Completed, 0));
            return json;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [GET TODOS] Failed to retrieve TODOs for session {SessionId}: {ErrorMessage}", 
                sessionId, ex.Message);
            
            string detailedError;
            if (ex is ArgumentException)
            {
                detailedError = $"Session with ID '{sessionId}' not found. Please verify the session ID is correct.";
            }
            else
            {
                detailedError = $"Failed to retrieve TODOs for session '{sessionId}': {ex.Message}";
            }
            
            var errorResult = new { 
                error = detailedError, 
                sessionId,
                exceptionType = ex.GetType().Name,
                stackTrace = ex.StackTrace
            };
            return JsonSerializer.Serialize(errorResult);
        }
    }

    /// <summary>
    /// Gets the current in-progress TODO item (first incomplete task) for a session via MCP
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <returns>JSON string containing the current in-progress TODO or null if all tasks are completed</returns>
    [McpServerTool]
    [Description("Gets the current in-progress TODO item for a session. Returns the first incomplete task (not completed) based on creation order.")]
    public async Task<string> GaiaTreePlannerGetCurrentInProgressTodoAsync(
        [Description("ID of the session to get the current in-progress TODO for, as from the start session reponse.")] string sessionId)
    {
        _logger.LogInformation("🎯 [GET CURRENT TODO] Finding current in-progress TODO for session {SessionId}...", sessionId);

        try
        {
            var currentTodo = await ((ITreePlannerManager)this).GetCurrentInProgressTodoAsync(sessionId);
            
            if (currentTodo == null)
            {
                _logger.LogInformation("✅ [GET CURRENT TODO] No incomplete TODOs found for session {SessionId} - all tasks completed!", sessionId);
                var noTodoResult = new { message = "All tasks completed", sessionId, currentTodo = (TodoItem?)null };
                return JsonSerializer.Serialize(noTodoResult);
            }

            var json = JsonSerializer.Serialize(currentTodo);
            var statusEmoji = currentTodo.Status switch
            {
                TodoStatus.Todo => "📋",
                TodoStatus.InProgress => "⏳",
                TodoStatus.Blocked => "🚫",
                _ => "❓"
            };
            
            _logger.LogInformation("🎯 [GET CURRENT TODO] Current in-progress TODO: {StatusEmoji} '{Title}' (ID: {TodoId}) | " +
                                 "Status: {Status} | Created: {CreatedAt:yyyy-MM-dd HH:mm:ss} UTC", 
                statusEmoji, currentTodo.Title, currentTodo.Id, currentTodo.Status, currentTodo.CreatedAt);
            return json;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [GET CURRENT TODO] Failed to get current in-progress TODO for session {SessionId}: {ErrorMessage}", 
                sessionId, ex.Message);
            
            string detailedError;
            if (ex is ArgumentException)
            {
                detailedError = $"Session with ID '{sessionId}' not found. Please verify the session ID is correct.";
            }
            else
            {
                detailedError = $"Failed to get current in-progress TODO for session '{sessionId}': {ex.Message}";
            }
            
            var errorResult = new { 
                error = detailedError, 
                sessionId,
                exceptionType = ex.GetType().Name,
                stackTrace = ex.StackTrace
            };
            return JsonSerializer.Serialize(errorResult);
        }
    }

    /// <summary>
    /// Gets the overall progress for a specific session via MCP
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <returns>JSON string containing progress information (total todos, completed todos, percentage)</returns>
    [McpServerTool]
    [Description("Gets the overall progress for a specific session / plan. Returns total TODOs, completed TODOs, and completion percentage.")]
    public async Task<string> GaiaTreePlannerGetSessionProgressAsync(
        [Description("ID of the session to get progress for, as from the start session reponse.")] string sessionId)
    {
        _logger.LogInformation("📊 [GET PROGRESS] Calculating progress for session {SessionId}...", sessionId);

        try
        {
            var progress = await ((ITreePlannerManager)this).GetSessionProgressAsync(sessionId);
            var json = JsonSerializer.Serialize(progress);
            
            _logger.LogInformation("✅ [GET PROGRESS] Session '{SessionName}' progress: " +
                                 "{CompletionPercentage}% complete ({CompletedTodos}/{TotalTodos} tasks) | " +
                                 "📋 {PendingTodos} pending, ⏳ {InProgressTodos} in progress", 
                progress.SessionName, progress.CompletionPercentage, progress.CompletedTodos, progress.TotalTodos,
                progress.PendingTodos, progress.InProgressTodos);
            return json;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [GET PROGRESS] Failed to calculate progress for session {SessionId}: {ErrorMessage}", 
                sessionId, ex.Message);
            
            string detailedError;
            if (ex is ArgumentException)
            {
                detailedError = $"Session with ID '{sessionId}' not found. Please verify the session ID is correct.";
            }
            else
            {
                detailedError = $"Failed to calculate progress for session '{sessionId}': {ex.Message}";
            }
            
            var errorResult = new { 
                error = detailedError, 
                sessionId,
                exceptionType = ex.GetType().Name,
                stackTrace = ex.StackTrace
            };
            return JsonSerializer.Serialize(errorResult);
        }
    }

    /// <summary>
    /// Starts a new project session
    /// </summary>
    /// <param name="projectName">Name of the project</param>
    /// <param name="description">Brief description that an AI can understand</param>
    /// <param name="aiAgentBuildContext">Concise context that will be needed for when the AI agent later uses the plan to build the solution</param>
    /// <param name="estimateHours">Estimated total hours for the project</param>
    /// <returns>Created project session</returns>
    async Task<ProjectPlan> ITreePlannerManager.StartNewSessionAsync(string projectName, string description, string aiAgentBuildContext, double estimateHours)
    {
        _logger.LogInformation("🏗️  [CORE] Creating project session '{ProjectName}'...", projectName);

        try
        {
            var session = new ProjectPlan
            {
                Name = projectName,
                Description = description,
                AiAgentBuildContext = aiAgentBuildContext,
                EstimateHours = estimateHours
            };

            var result = await _repository.AddSessionAsync(session);
            _logger.LogInformation("✅ [CORE] Project session '{ProjectName}' created with ID: {SessionId} at {CreatedAt}", 
                projectName, result.Id, result.CreatedAt);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [CORE] Failed to create project session '{ProjectName}': {ErrorMessage}", 
                projectName, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Adds a new TODO item to a session
    /// </summary>
    /// <param name="sessionId">ID of the project session</param>
    /// <param name="title">Title/description of the TODO</param>
    /// <param name="description">Detailed description with acceptance criteria</param>
    /// <param name="tags">Tags for grouping</param>
    /// <param name="groups">Groups for organizing TODOs</param>
    /// <param name="parentTodoId">ID of parent TODO if this is nested</param>
    /// <param name="estimateHours">Estimated hours for this TODO</param>
    /// <returns>Created TODO item</returns>
    async Task<TodoItem> ITreePlannerManager.AddTodoAsync(string sessionId, string title, string description, List<string>? tags, List<string>? groups, string? parentTodoId, double estimateHours)
    {
        var nestingInfo = !string.IsNullOrEmpty(parentTodoId) ? $" as child of {parentTodoId}" : " at root level";
        _logger.LogInformation("🏗️  [CORE] Adding TODO '{Title}' to session {SessionId}{NestingInfo}", 
            title, sessionId, nestingInfo);

        try
        {
            var todo = new TodoItem
            {
                SessionId = sessionId,
                Title = title,
                Description = description,
                Tags = tags ?? new List<string>(),
                Groups = groups ?? new List<string>(),
                ParentTodoId = parentTodoId,
                Status = TodoStatus.Todo,
                EstimateHours = estimateHours
            };

            await _repository.AddTodoAsync(todo);
            _logger.LogInformation("✅ [CORE] TODO '{Title}' created with ID: {TodoId} | Tags: [{Tags}] | Groups: [{Groups}]", 
                title, todo.Id, 
                string.Join(", ", todo.Tags), 
                string.Join(", ", todo.Groups));
            return todo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [CORE] Failed to add TODO '{Title}' to session {SessionId}: {ErrorMessage}", 
                title, sessionId, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Updates parent TODO status if all its children are completed
    /// </summary>
    /// <param name="parentTodoId">ID of the parent TODO to check</param>
    /// <param name="sessionId">ID of the session</param>
    private async Task UpdateParentTodoStatusIfNeededAsync(string parentTodoId, string sessionId)
    {
        try
        {
            var allTodos = await _repository.GetTodosBySessionAsync(sessionId);
            var parentTodo = allTodos.FirstOrDefault(t => t.Id == parentTodoId);
            
            if (parentTodo == null)
            {
                _logger.LogWarning("⚠️  [PARENT UPDATE] Parent TODO {ParentTodoId} not found in session {SessionId}", 
                    parentTodoId, sessionId);
                return;
            }

            // Skip if parent is already completed
            if (parentTodo.Status == TodoStatus.Completed)
            {
                _logger.LogInformation("✅ [PARENT UPDATE] Parent TODO '{ParentTitle}' is already completed", parentTodo.Title);
                return;
            }

            // Get all children of this parent
            var children = allTodos.Where(t => t.ParentTodoId == parentTodoId).ToList();
            
            // If no children, nothing to do
            if (!children.Any())
            {
                _logger.LogInformation("📝 [PARENT UPDATE] Parent TODO '{ParentTitle}' has no children", parentTodo.Title);
                return;
            }

            // Check if all children are completed
            var allChildrenCompleted = children.All(c => c.Status == TodoStatus.Completed);
            
            if (allChildrenCompleted)
            {
                _logger.LogInformation("🎉 [PARENT UPDATE] All {ChildCount} children of '{ParentTitle}' completed! " +
                                     "Auto-completing parent TODO {ParentTodoId}", 
                    children.Count, parentTodo.Title, parentTodoId);
                
                // Update parent status to completed
                parentTodo.Status = TodoStatus.Completed;
                await _repository.UpdateTodoAsync(parentTodo);
                
                // Recursively check if this parent has a parent that should also be updated
                if (!string.IsNullOrWhiteSpace(parentTodo.ParentTodoId))
                {
                    _logger.LogInformation("🔄 [PARENT UPDATE] Checking grandparent TODO {GrandparentTodoId}", 
                        parentTodo.ParentTodoId);
                    await UpdateParentTodoStatusIfNeededAsync(parentTodo.ParentTodoId, sessionId);
                }
            }
            else
            {
                var completedCount = children.Count(c => c.Status == TodoStatus.Completed);
                _logger.LogInformation("⏳ [PARENT UPDATE] Parent TODO '{ParentTitle}' progress: {CompletedCount}/{TotalCount} children completed", 
                    parentTodo.Title, completedCount, children.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [PARENT UPDATE] Error updating parent TODO {ParentTodoId} in session {SessionId}: {ErrorMessage}", 
                parentTodoId, sessionId, ex.Message);
            // Don't throw here to avoid breaking the main update operation
        }
    }

    /// <summary>
    /// Updates the status of a TODO item
    /// </summary>
    /// <param name="todoId">ID of the TODO item to update</param>
    /// <param name="newStatus">New status for the TODO</param>
    /// <returns>Updated TODO item</returns>
    async Task<TodoItem> ITreePlannerManager.UpdateTodoStatusAsync(string todoId, TodoStatus newStatus)
    {
        var statusEmoji = newStatus switch
        {
            TodoStatus.Todo => "📋",
            TodoStatus.InProgress => "⏳",
            TodoStatus.Completed => "✅",
            _ => "❓"
        };
        
        _logger.LogInformation("🔄 [CORE] Updating TODO {TodoId} status to {StatusEmoji} {NewStatus}", 
            todoId, statusEmoji, newStatus);

        try
        {
            // Update the status using the repository's status update method
            await _repository.UpdateTodoStatusAsync(todoId, newStatus);
            
            // Get the updated TODO to return
            var updatedTodo = await _repository.GetTodoByIdAsync(todoId);
            
            if (updatedTodo == null)
            {
                var errorMessage = $"TODO with ID '{todoId}' was not found after status update. This indicates a critical data consistency issue - the TODO may have been deleted or the database may be corrupted.";
                _logger.LogError("💀 [CORE] Critical error: {ErrorMessage}", errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            
            _logger.LogInformation("✅ [CORE] TODO '{Title}' status updated to {StatusEmoji} {NewStatus}", 
                updatedTodo.Title, statusEmoji, newStatus);
            
            return updatedTodo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [CORE] Failed to update TODO {TodoId} status to {NewStatus}: {ErrorMessage}", 
                todoId, newStatus, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets all project sessions
    /// </summary>
    /// <returns>List of all project sessions</returns>
    async Task<List<ProjectPlan>> ITreePlannerManager.GetAllSessionsAsync()
    {
        _logger.LogInformation("🏗️  [CORE] Retrieving all project sessions from repository...");

        try
        {
            var sessions = await _repository.GetAllSessionsAsync();
            _logger.LogInformation("✅ [CORE] Retrieved {Count} project session(s): {SessionList}", 
                sessions.Count, 
                sessions.Any() ? string.Join(", ", sessions.Select(s => $"'{s.Name}' ({s.Id})")) : "none");
            return sessions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [CORE] Failed to retrieve project sessions: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets all TODO items for a specific session
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <returns>Project session with all TODO items</returns>
    async Task<ProjectPlan> ITreePlannerManager.GetTodosBySessionAsync(string sessionId)
    {
        _logger.LogInformation("🏗️  [CORE] Loading session {SessionId} with all TODOs...", sessionId);

        try
        {
            // Get the session with hierarchical TODOs already populated
            var session = await _repository.GetSessionByIdAsync(sessionId);
            
            if (session == null)
            {
                throw new ArgumentException($"Session with ID {sessionId} not found");
            }

            // Get flat list for counting/statistics
            var flatTodos = await _repository.GetTodosBySessionAsync(sessionId);
            
            var statusBreakdown = flatTodos.GroupBy(t => t.Status)
                .ToDictionary(g => g.Key, g => g.Count());
            
            _logger.LogInformation("✅ [CORE] Loaded session '{SessionName}' with {Count} TODO(s) | " +
                                 "📋 {TodoCount} pending, ⏳ {InProgressCount} in progress, ✅ {CompletedCount} completed", 
                session.Name, flatTodos.Count,
                statusBreakdown.GetValueOrDefault(TodoStatus.Todo, 0),
                statusBreakdown.GetValueOrDefault(TodoStatus.InProgress, 0),
                statusBreakdown.GetValueOrDefault(TodoStatus.Completed, 0));
            
            return session;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [CORE] Failed to load session {SessionId} with TODOs: {ErrorMessage}", 
                sessionId, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets the overall progress for a specific session
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <returns>Progress information including total todos, completed todos, and percentage</returns>
    async Task<SessionProgress> ITreePlannerManager.GetSessionProgressAsync(string sessionId)
    {
        _logger.LogInformation("🏗️  [CORE] Calculating progress statistics for session {SessionId}...", sessionId);

        try
        {
            // Get the session
            var session = await _repository.GetSessionByIdAsync(sessionId);

            // Get all TODOs for the session
            var todos = await _repository.GetTodosBySessionAsync(sessionId);
            
            // Calculate progress statistics
            var totalTodos = todos.Count;
            var completedTodos = todos.Count(t => t.Status == TodoStatus.Completed);
            var inProgressTodos = todos.Count(t => t.Status == TodoStatus.InProgress);
            var pendingTodos = todos.Count(t => t.Status == TodoStatus.Todo);
            
            // Calculate estimate hours by status
            var totalTodoEstimateHours = todos.Sum(t => t.EstimateHours);
            var completedTodoEstimateHours = todos.Where(t => t.Status == TodoStatus.Completed).Sum(t => t.EstimateHours);
            var inProgressTodoEstimateHours = todos.Where(t => t.Status == TodoStatus.InProgress).Sum(t => t.EstimateHours);
            var pendingTodoEstimateHours = todos.Where(t => t.Status == TodoStatus.Todo).Sum(t => t.EstimateHours);
            
            // Calculate completion percentages
            var completionPercentage = totalTodos > 0 ? Math.Round((double)completedTodos / totalTodos * 100, 2) : 0.0;
            var estimateCompletionPercentage = totalTodoEstimateHours > 0 ? Math.Round(completedTodoEstimateHours / totalTodoEstimateHours * 100, 2) : 0.0;
            
            var progress = new SessionProgress
            {
                SessionId = sessionId,
                SessionName = session!.Name,
                TotalTodos = totalTodos,
                CompletedTodos = completedTodos,
                InProgressTodos = inProgressTodos,
                PendingTodos = pendingTodos,
                CompletionPercentage = completionPercentage,
                ProjectEstimateHours = session.EstimateHours,
                TotalTodoEstimateHours = totalTodoEstimateHours,
                CompletedTodoEstimateHours = completedTodoEstimateHours,
                InProgressTodoEstimateHours = inProgressTodoEstimateHours,
                PendingTodoEstimateHours = pendingTodoEstimateHours,
                EstimateCompletionPercentage = estimateCompletionPercentage
            };
            
            var progressBar = GenerateProgressBar(completionPercentage);
            var estimateProgressBar = GenerateProgressBar(estimateCompletionPercentage);
            _logger.LogInformation("✅ [CORE] Progress calculated for '{SessionName}':\n" +
                                 "   📊 Tasks: {ProgressBar} {CompletionPercentage}% | {CompletedTodos}/{TotalTodos} completed\n" +
                                 "   ⏱️ Hours: {EstimateProgressBar} {EstimateCompletionPercentage}% | {CompletedHours:F1}/{TotalHours:F1}h completed\n" +
                                 "   📋 {PendingTodos} pending ({PendingHours:F1}h), ⏳ {InProgressTodos} in progress ({InProgressHours:F1}h)\n" +
                                 "   🎯 Project estimate: {ProjectEstimate:F1}h vs TODOs: {TodoEstimate:F1}h", 
                session.Name, progressBar, completionPercentage, completedTodos, totalTodos, 
                estimateProgressBar, estimateCompletionPercentage, completedTodoEstimateHours, totalTodoEstimateHours,
                pendingTodos, pendingTodoEstimateHours, inProgressTodos, inProgressTodoEstimateHours,
                session.EstimateHours, totalTodoEstimateHours);
            return progress;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [CORE] Failed to calculate progress for session {SessionId}: {ErrorMessage}", 
                sessionId, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets the current in-progress TODO item (first incomplete task) for a session
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <returns>Current in-progress TODO or null if all tasks are completed</returns>
    async Task<TodoItem?> ITreePlannerManager.GetCurrentInProgressTodoAsync(string sessionId)
    {
        _logger.LogInformation("🏗️  [CORE] Finding current in-progress TODO for session {SessionId}...", sessionId);

        try
        {
            // Get all TODOs for the session (flat list for easier processing)
            var todos = await _repository.GetTodosBySessionAsync(sessionId);
            
            if (!todos.Any())
            {
                _logger.LogInformation("📭 [CORE] No TODOs found in session {SessionId}", sessionId);
                return null;
            }

            // Find the first incomplete TODO ordered by creation date
            // Priority: TodoStatus.InProgress first, then TodoStatus.Todo, then TodoStatus.Blocked
            var currentTodo = todos
                .Where(t => t.Status != TodoStatus.Completed && t.Status != TodoStatus.Cancelled)
                .OrderBy(t => t.Status == TodoStatus.InProgress ? 0 : 
                            t.Status == TodoStatus.Todo ? 1 : 
                            t.Status == TodoStatus.Blocked ? 2 : 3)
                .ThenBy(t => t.CreatedAt)
                .FirstOrDefault();
            
            if (currentTodo == null)
            {
                _logger.LogInformation("🎉 [CORE] All TODOs in session {SessionId} are completed! 🎉", sessionId);
                return null;
            }

            var statusEmoji = currentTodo.Status switch
            {
                TodoStatus.Todo => "📋",
                TodoStatus.InProgress => "⏳",
                TodoStatus.Blocked => "🚫",
                _ => "❓"
            };

            var nestingInfo = !string.IsNullOrEmpty(currentTodo.ParentTodoId) ? $" (child of {currentTodo.ParentTodoId})" : " (root-level)";
            _logger.LogInformation("🎯 [CORE] Current in-progress TODO found: {StatusEmoji} '{Title}' | " +
                                 "Status: {Status} | ID: {TodoId}{NestingInfo} | " +
                                 "Created: {CreatedAt:yyyy-MM-dd HH:mm:ss} UTC | " +
                                 "Estimate: {EstimateHours}h",
                statusEmoji, currentTodo.Title, currentTodo.Status, currentTodo.Id, nestingInfo,
                currentTodo.CreatedAt, currentTodo.EstimateHours);

            return currentTodo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [CORE] Failed to get current in-progress TODO for session {SessionId}: {ErrorMessage}", 
                sessionId, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Generates a visual progress bar for logging
    /// </summary>
    /// <param name="percentage">Completion percentage</param>
    /// <returns>Visual progress bar string</returns>
    private static string GenerateProgressBar(double percentage)
    {
        const int barLength = 20;
        var filledLength = (int)(percentage / 100.0 * barLength);
        var filled = new string('█', filledLength);
        var empty = new string('░', barLength - filledLength);
        return $"[{filled}{empty}]";
    }
}
