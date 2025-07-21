using FrostAura.MCP.Gaia.Models;
using FrostAura.MCP.Gaia.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FrostAura.MCP.Gaia.Data;

/// <summary>
/// JSON database context for TODO data with hierarchical structure
/// </summary>
public class TreePlannerDbContext
{
    private readonly ILogger<TreePlannerDbContext> _logger;
    private readonly string _databasePath;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the TreePlannerDbContext
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="configuration">Configuration instance</param>
    public TreePlannerDbContext(ILogger<TreePlannerDbContext> logger, IConfiguration configuration)
    {
        _logger = logger;
        _databasePath = configuration["TreePlanner:DatabasePath"] ?? throw new ArgumentNullException("TreePlanner:DatabasePath configuration is required");
        _jsonOptions = new JsonSerializerOptions
        {
            // Use PascalCase to match C# property names exactly - fixes TODO persistence
            PropertyNamingPolicy = null, 
            WriteIndented = true
        };
    }

    /// <summary>
    /// Represents the database structure - plans with nested todos
    /// </summary>
    private class TodoDatabase
    {
        public List<ProjectPlan> Sessions { get; set; } = new List<ProjectPlan>();
    }

    /// <summary>
    /// Gets the database content
    /// </summary>
    /// <returns>Database content</returns>
    private async Task<TodoDatabase> GetDatabaseAsync()
    {
        _logger.LogInformation("🗄️  [DB] Loading database from: {DatabasePath}", _databasePath);

        try
        {
            if (!File.Exists(_databasePath))
            {
                _logger.LogInformation("🗄️  [DB] Database file does not exist, creating new empty database");
                await CreateDatabaseFileAsync();
                return new TodoDatabase();
            }

            var jsonContent = await File.ReadAllTextAsync(_databasePath);
            
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                _logger.LogInformation("🗄️  [DB] Database file is empty, returning empty database");
                return new TodoDatabase();
            }

            // Check if it's the old format (with separate todos array)
            var isOldFormat = jsonContent.Contains("\"todos\":");
            
            if (isOldFormat)
            {
                _logger.LogInformation("🔄 [DB] Detected old database format, migrating to hierarchical structure...");
                var oldDatabase = JsonSerializer.Deserialize<OldTodoDatabase>(jsonContent, _jsonOptions);
                var newDatabase = MigrateFromOldFormat(oldDatabase!);
                await SaveDatabaseAsync(newDatabase);
                return newDatabase;
            }

            var database = JsonSerializer.Deserialize<TodoDatabase>(jsonContent, _jsonOptions);
            
            // Ensure all TODOs have properly initialized Children lists after deserialization
            if (database?.Sessions != null)
            {
                foreach (var session in database.Sessions)
                {
                    if (session.Todos != null)
                    {
                        EnsureChildrenInitialized(session.Todos);
                    }
                }
            }
            
            var totalTodos = CountAllTodos(database?.Sessions ?? new List<ProjectPlan>());
            _logger.LogInformation("✅ [DB] Successfully loaded database with {SessionCount} sessions and {TodoCount} todos", 
                database?.Sessions?.Count ?? 0, totalTodos);
            
            return database ?? new TodoDatabase();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [DB] Error loading database");
            return new TodoDatabase();
        }
    }

    /// <summary>
    /// Gets all TODOs flattened from all sessions for debugging
    /// </summary>
    /// <param name="sessions">Sessions to get TODOs from</param>
    /// <returns>Flat list of all TODOs</returns>
    private List<TodoItem> GetAllTodosFlat(List<ProjectPlan> sessions)
    {
        var result = new List<TodoItem>();
        foreach (var session in sessions)
        {
            result.AddRange(FlattenTodos(session.Todos));
        }
        return result;
    }

    /// <summary>
    /// Ensures all TODOs in a list have properly initialized Children lists (recursively)
    /// </summary>
    /// <param name="todos">List of TODOs to initialize</param>
    private void EnsureChildrenInitialized(List<TodoItem> todos)
    {
        foreach (var todo in todos)
        {
            if (todo.Children == null)
            {
                todo.Children = new List<TodoItem>();
            }
            EnsureChildrenInitialized(todo.Children);
        }
    }

    /// <summary>
    /// Old database format for migration
    /// </summary>
    private class OldTodoDatabase
    {
        public List<ProjectPlan> Sessions { get; set; } = new List<ProjectPlan>();
        public List<TodoItem> Todos { get; set; } = new List<TodoItem>();
    }

    /// <summary>
    /// Migrates old flat format to new hierarchical format
    /// </summary>
    /// <param name="oldDatabase">Old database format</param>
    /// <returns>New hierarchical database</returns>
    private TodoDatabase MigrateFromOldFormat(OldTodoDatabase oldDatabase)
    {
        _logger.LogInformation("🔄 [MIGRATION] Starting migration from flat to hierarchical format");
        
        var newDatabase = new TodoDatabase
        {
            Sessions = new List<ProjectPlan>()
        };

        foreach (var session in oldDatabase.Sessions)
        {
            // Get all todos for this session
            var sessionTodos = oldDatabase.Todos.Where(t => t.SessionId == session.Id).ToList();
            
            // Build hierarchy
            var hierarchicalTodos = BuildTodoHierarchy(sessionTodos);
            
            // Update session with hierarchical todos
            session.Todos = hierarchicalTodos;
            newDatabase.Sessions.Add(session);
        }

        var totalTodos = CountAllTodos(newDatabase.Sessions);
        _logger.LogInformation("✅ [MIGRATION] Migration completed: {SessionCount} sessions, {TodoCount} todos", 
            newDatabase.Sessions.Count, totalTodos);
        
        return newDatabase;
    }

    /// <summary>
    /// Builds hierarchical todo structure from flat list
    /// </summary>
    /// <param name="flatTodos">Flat list of todos</param>
    /// <returns>Hierarchical todo structure</returns>
    private List<TodoItem> BuildTodoHierarchy(List<TodoItem> flatTodos)
    {
        var todoDict = flatTodos.ToDictionary(t => t.Id, t => t);
        var rootTodos = new List<TodoItem>();

        foreach (var todo in flatTodos)
        {
            todo.Children = new List<TodoItem>();
            
            if (string.IsNullOrEmpty(todo.ParentTodoId))
            {
                rootTodos.Add(todo);
            }
            else if (todoDict.TryGetValue(todo.ParentTodoId, out var parent))
            {
                parent.Children.Add(todo);
            }
            else
            {
                // Parent not found, treat as root
                _logger.LogWarning("⚠️  [MIGRATION] Parent todo {ParentId} not found for todo {TodoId}, treating as root", 
                    todo.ParentTodoId, todo.Id);
                rootTodos.Add(todo);
            }
        }

        return rootTodos;
    }

    /// <summary>
    /// Counts all todos recursively
    /// </summary>
    /// <param name="sessions">Sessions to count todos for</param>
    /// <returns>Total todo count</returns>
    private int CountAllTodos(List<ProjectPlan> sessions)
    {
        return sessions.Sum(s => CountTodosRecursive(s.Todos));
    }

    /// <summary>
    /// Counts todos recursively including children
    /// </summary>
    /// <param name="todos">Todos to count</param>
    /// <returns>Total count including children</returns>
    private int CountTodosRecursive(List<TodoItem> todos)
    {
        return todos.Count + todos.Sum(t => CountTodosRecursive(t.Children));
    }

    /// <summary>
    /// Saves the database content
    /// </summary>
    /// <param name="database">Database content to save</param>
    private async Task SaveDatabaseAsync(TodoDatabase database)
    {
        var totalTodos = CountAllTodos(database.Sessions);
        _logger.LogInformation("💾 [DB] Saving database with {SessionCount} sessions and {TodoCount} todos", 
            database.Sessions.Count, totalTodos);

        try
        {
            var jsonContent = JsonSerializer.Serialize(database, _jsonOptions);
            await File.WriteAllTextAsync(_databasePath, jsonContent);
            _logger.LogInformation("✅ [DB] Successfully saved database");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [DB] Error saving database");
            throw;
        }
    }

    /// <summary>
    /// Creates an empty database file
    /// </summary>
    private async Task CreateDatabaseFileAsync()
    {
        _logger.LogInformation("🗄️  [DB] Creating new database file: {DatabasePath}", _databasePath);

        try
        {
            var emptyDatabase = new TodoDatabase();
            await SaveDatabaseAsync(emptyDatabase);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [DB] Error creating database file");
            throw;
        }
    }

    /// <summary>
    /// Adds a new project session
    /// </summary>
    /// <param name="session">Project session to add</param>
    public async Task AddSessionAsync(ProjectPlan session)
    {
        _logger.LogInformation("🗄️  [DB] Adding new project session: {Name}", session.Name);

        try
        {
            var database = await GetDatabaseAsync();
            
            // Ensure unique ID
            if (database.Sessions.Any(s => s.Id == session.Id))
            {
                session.Id = Guid.NewGuid().ToString();
                _logger.LogInformation("🗄️  [DB] Generated new unique session ID: {Id}", session.Id);
            }

            // Set timestamps
            session.CreatedAt = DateTime.UtcNow;
            session.UpdatedAt = DateTime.UtcNow;
            session.Todos = new List<TodoItem>(); // Ensure empty todos list

            database.Sessions.Add(session);
            await SaveDatabaseAsync(database);
            
            _logger.LogInformation("✅ [DB] Successfully added project session with ID: {Id}", session.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [DB] Error adding project session");
            throw;
        }
    }

    /// <summary>
    /// Adds a new TODO item to the appropriate session and parent
    /// </summary>
    /// <param name="todo">TODO item to add</param>
    public async Task AddTodoAsync(TodoItem todo)
    {
        _logger.LogInformation("🗄️  [DB] Adding new TODO item: {Title}", todo.Title);

        try
        {
            var database = await GetDatabaseAsync();
            
            // Find the session
            var session = database.Sessions.FirstOrDefault(s => s.Id == todo.SessionId);
            if (session == null)
            {
                throw new ArgumentException($"Session with ID {todo.SessionId} not found");
            }

            // Ensure unique ID across all todos
            if (FindTodoById(database.Sessions, todo.Id) != null)
            {
                todo.Id = Guid.NewGuid().ToString();
                _logger.LogInformation("🗄️  [DB] Generated new unique TODO ID: {Id}", todo.Id);
            }

            // Set timestamps
            todo.CreatedAt = DateTime.UtcNow;
            todo.UpdatedAt = DateTime.UtcNow;
            todo.Children = new List<TodoItem>(); // Ensure empty children list

            // Add to appropriate parent or session root
            if (string.IsNullOrEmpty(todo.ParentTodoId))
            {
                session.Todos.Add(todo);
                _logger.LogInformation("🗄️  [DB] Added TODO as root-level item in session {SessionId}. Session now has {TodoCount} todos", 
                    session.Id, session.Todos.Count);
            }
            else
            {
                var parent = FindTodoById(database.Sessions, todo.ParentTodoId);
                if (parent == null)
                {
                    _logger.LogError("❌ [DB] Parent TODO with ID {ParentTodoId} not found. Available todos: {AvailableTodos}", 
                        todo.ParentTodoId, 
                        string.Join(", ", GetAllTodosFlat(database.Sessions).Select(t => $"{t.Title}({t.Id})")));
                    throw new ArgumentException($"Parent TODO with ID {todo.ParentTodoId} not found");
                }
                parent.Children.Add(todo);
                _logger.LogInformation("🗄️  [DB] Added TODO as child of {ParentTitle} ({ParentId})", parent.Title, parent.Id);
            }

            // Update the session's UpdatedAt timestamp
            session.UpdatedAt = DateTime.UtcNow;

            await SaveDatabaseAsync(database);
            
            _logger.LogInformation("✅ [DB] Successfully added TODO item with ID: {Id}", todo.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [DB] Error adding TODO item");
            throw;
        }
    }

    /// <summary>
    /// Finds a TODO by ID recursively across all sessions and nested todos
    /// </summary>
    /// <param name="sessions">Sessions to search</param>
    /// <param name="todoId">ID to find</param>
    /// <returns>Found TODO or null</returns>
    private TodoItem? FindTodoById(List<ProjectPlan> sessions, string todoId)
    {
        foreach (var session in sessions)
        {
            var found = FindTodoByIdRecursive(session.Todos, todoId);
            if (found != null) return found;
        }
        return null;
    }

    /// <summary>
    /// Finds a TODO by ID recursively in a todo list
    /// </summary>
    /// <param name="todos">Todos to search</param>
    /// <param name="todoId">ID to find</param>
    /// <returns>Found TODO or null</returns>
    private TodoItem? FindTodoByIdRecursive(List<TodoItem> todos, string todoId)
    {
        foreach (var todo in todos)
        {
            if (todo.Id == todoId) return todo;
            
            var found = FindTodoByIdRecursive(todo.Children, todoId);
            if (found != null) return found;
        }
        return null;
    }

    /// <summary>
    /// Gets a project session by ID
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <returns>Project session or null if not found</returns>
    public async Task<ProjectPlan?> GetSessionByIdAsync(string sessionId)
    {
        var database = await GetDatabaseAsync();
        return database.Sessions.FirstOrDefault(s => s.Id == sessionId);
    }

    /// <summary>
    /// Gets all project sessions
    /// </summary>
    /// <returns>List of all project sessions</returns>
    public async Task<List<ProjectPlan>> GetAllSessionsAsync()
    {
        var database = await GetDatabaseAsync();
        return database.Sessions.ToList();
    }

    /// <summary>
    /// Gets all TODO items for a session (flattened for backwards compatibility)
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <returns>List of TODO items for the session</returns>
    public async Task<List<TodoItem>> GetTodosBySessionAsync(string sessionId)
    {
        var database = await GetDatabaseAsync();
        var session = database.Sessions.FirstOrDefault(s => s.Id == sessionId);
        if (session == null) return new List<TodoItem>();
        
        return FlattenTodos(session.Todos);
    }

    /// <summary>
    /// Flattens hierarchical todos to a flat list
    /// </summary>
    /// <param name="todos">Hierarchical todos</param>
    /// <returns>Flat list of todos</returns>
    private List<TodoItem> FlattenTodos(List<TodoItem> todos)
    {
        var result = new List<TodoItem>();
        
        foreach (var todo in todos)
        {
            result.Add(todo);
            result.AddRange(FlattenTodos(todo.Children));
        }
        
        return result;
    }

    /// <summary>
    /// Gets all TODO items across all sessions (flattened)
    /// </summary>
    /// <returns>List of all TODO items</returns>
    public async Task<List<TodoItem>> GetAllTodosAsync()
    {
        var database = await GetDatabaseAsync();
        var allTodos = new List<TodoItem>();
        
        foreach (var session in database.Sessions)
        {
            allTodos.AddRange(FlattenTodos(session.Todos));
        }
        
        return allTodos;
    }

    /// <summary>
    /// Gets a TODO item by ID
    /// </summary>
    /// <param name="todoId">ID of the TODO item</param>
    /// <returns>TODO item or null if not found</returns>
    public async Task<TodoItem?> GetTodoByIdAsync(string todoId)
    {
        _logger.LogInformation("🗄️  [DB] Getting TODO item by ID: {TodoId}", todoId);

        try
        {
            var database = await GetDatabaseAsync();
            return FindTodoById(database.Sessions, todoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [DB] Error getting TODO item: {TodoId}", todoId);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing TODO item
    /// </summary>
    /// <param name="todo">TODO item to update</param>
    public async Task UpdateTodoAsync(TodoItem todo)
    {
        _logger.LogInformation("🗄️  [DB] Updating TODO item: {TodoId}", todo.Id);

        try
        {
            var database = await GetDatabaseAsync();
            var existingTodo = FindTodoById(database.Sessions, todo.Id);
            
            if (existingTodo == null)
            {
                throw new ArgumentException($"TODO with ID {todo.Id} not found");
            }

            // Update properties but preserve hierarchy
            existingTodo.Title = todo.Title;
            existingTodo.Description = todo.Description;
            existingTodo.Status = todo.Status;
            existingTodo.Tags = todo.Tags;
            existingTodo.Groups = todo.Groups;
            existingTodo.UpdatedAt = DateTime.UtcNow;

            // Handle completion timestamp
            if (todo.Status == TodoStatus.Completed && existingTodo.CompletedAt == null)
            {
                existingTodo.CompletedAt = DateTime.UtcNow;
                _logger.LogInformation("🗄️  [DB] TODO {TodoId} marked as completed", todo.Id);
            }
            else if (todo.Status != TodoStatus.Completed && existingTodo.CompletedAt != null)
            {
                existingTodo.CompletedAt = null;
                _logger.LogInformation("🗄️  [DB] TODO {TodoId} moved away from completed status, cleared CompletedAt", todo.Id);
            }

            // Update the session's UpdatedAt timestamp
            var session = database.Sessions.FirstOrDefault(s => s.Id == existingTodo.SessionId);
            if (session != null)
            {
                session.UpdatedAt = DateTime.UtcNow;
            }

            await SaveDatabaseAsync(database);
            _logger.LogInformation("✅ [DB] Successfully updated TODO item: {TodoId}", todo.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [DB] Error updating TODO item: {TodoId}", todo.Id);
            throw;
        }
    }

    /// <summary>
    /// Updates the status of a TODO item with hierarchical completion logic
    /// </summary>
    /// <param name="todoId">ID of the TODO to update</param>
    /// <param name="newStatus">New status</param>
    public async Task UpdateTodoStatusAsync(string todoId, TodoStatus newStatus)
    {
        _logger.LogInformation("🗄️  [DB] Updating TODO status: {TodoId} to {Status}", todoId, newStatus);

        try
        {
            var database = await GetDatabaseAsync();
            var todo = FindTodoById(database.Sessions, todoId);
            
            if (todo == null)
            {
                throw new ArgumentException($"TODO with ID {todoId} not found");
            }

            // Update the status
            var oldStatus = todo.Status;
            todo.Status = newStatus;
            todo.UpdatedAt = DateTime.UtcNow;

            // Handle completion timestamp
            if (newStatus == TodoStatus.Completed && todo.CompletedAt == null)
            {
                todo.CompletedAt = DateTime.UtcNow;
                _logger.LogInformation("🗄️  [DB] TODO {TodoId} marked as completed", todoId);
            }
            else if (newStatus != TodoStatus.Completed && todo.CompletedAt != null)
            {
                todo.CompletedAt = null;
                _logger.LogInformation("🗄️  [DB] TODO {TodoId} moved away from completed status, cleared CompletedAt", todoId);
            }

            // Hierarchical completion logic: if this TODO is now completed, check if parent should auto-complete
            if (newStatus == TodoStatus.Completed && oldStatus != TodoStatus.Completed)
            {
                await CheckAndUpdateParentCompletionAsync(database, todo);
            }

            // Update the session's UpdatedAt timestamp
            var session = database.Sessions.FirstOrDefault(s => s.Id == todo.SessionId);
            if (session != null)
            {
                session.UpdatedAt = DateTime.UtcNow;
            }

            await SaveDatabaseAsync(database);
            _logger.LogInformation("✅ [DB] Successfully updated TODO status: {TodoId} to {Status}", todoId, newStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [DB] Error updating TODO status: {TodoId}", todoId);
            throw;
        }
    }

    /// <summary>
    /// Checks if a parent TODO should be automatically completed when all children are done
    /// </summary>
    /// <param name="database">Database instance</param>
    /// <param name="childTodo">The child TODO that was just completed</param>
    private async Task CheckAndUpdateParentCompletionAsync(TodoDatabase database, TodoItem childTodo)
    {
        if (string.IsNullOrEmpty(childTodo.ParentTodoId))
        {
            _logger.LogInformation("🔍 [DB] TODO {TodoId} has no parent, skipping hierarchical completion check", childTodo.Id);
            return;
        }

        var parentTodo = FindTodoById(database.Sessions, childTodo.ParentTodoId);
        if (parentTodo == null)
        {
            _logger.LogWarning("⚠️  [DB] Parent TODO {ParentId} not found for child {ChildId}", childTodo.ParentTodoId, childTodo.Id);
            return;
        }

        // Check if all children of the parent are completed
        var allChildrenCompleted = parentTodo.Children.All(child => child.Status == TodoStatus.Completed);
        
        if (allChildrenCompleted && parentTodo.Status != TodoStatus.Completed)
        {
            _logger.LogInformation("🎯 [DB] All children of parent {ParentId} are completed, auto-completing parent", parentTodo.Id);
            
            parentTodo.Status = TodoStatus.Completed;
            parentTodo.CompletedAt = DateTime.UtcNow;
            parentTodo.UpdatedAt = DateTime.UtcNow;
            
            // Recursively check if this parent's parent should also be completed
            await CheckAndUpdateParentCompletionAsync(database, parentTodo);
        }
        else
        {
            _logger.LogInformation("📋 [DB] Parent {ParentId} still has incomplete children, not auto-completing", parentTodo.Id);
        }
    }
}
