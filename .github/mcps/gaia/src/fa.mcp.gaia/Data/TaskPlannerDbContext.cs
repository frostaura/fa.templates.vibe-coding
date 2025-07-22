using FrostAura.MCP.Gaia.Models;
using FrostAura.MCP.Gaia.Enums;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace FrostAura.MCP.Gaia.Data;

/// <summary>
/// JSON database context for Task data with hierarchical structure
/// </summary>
public class TaskPlannerDbContext
{
    private readonly string _databasePath;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the TaskPlannerDbContext
    /// </summary>
    /// <param name="configuration">Configuration instance</param>
    public TaskPlannerDbContext(IConfiguration configuration)
    {
        _databasePath = configuration["TaskPlanner:DatabasePath"] ?? throw new ArgumentNullException("TaskPlanner:DatabasePath configuration is required");
        _jsonOptions = new JsonSerializerOptions
        {
            // Use PascalCase to match C# property names exactly - fixes Task persistence
            PropertyNamingPolicy = null, 
            WriteIndented = true
        };
    }

    /// <summary>
    /// Represents the database structure - plans with tasks
    /// </summary>
    private class TaskDatabase
    {
        public List<ProjectPlan> Plans { get; set; } = new List<ProjectPlan>();
        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }

    /// <summary>
    /// Gets the database content
    /// </summary>
    /// <returns>Database content</returns>
    private async Task<TaskDatabase> GetDatabaseAsync()
    {
        if (!File.Exists(_databasePath))
        {
            await CreateDatabaseFileAsync();
            return new TaskDatabase();
        }

        var jsonContent = await File.ReadAllTextAsync(_databasePath);
        
        if (string.IsNullOrWhiteSpace(jsonContent))
        {
            return new TaskDatabase();
        }

        var database = JsonSerializer.Deserialize<TaskDatabase>(jsonContent, _jsonOptions);
        return database ?? new TaskDatabase();
    }

    /// <summary>
    /// Saves the database content
    /// </summary>
    /// <param name="database">Database content to save</param>
    private async Task SaveDatabaseAsync(TaskDatabase database)
    {
        // Ensure directory exists
        var directory = Path.GetDirectoryName(_databasePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var jsonContent = JsonSerializer.Serialize(database, _jsonOptions);
        await File.WriteAllTextAsync(_databasePath, jsonContent);
    }

    /// <summary>
    /// Creates an empty database file
    /// </summary>
    private async Task CreateDatabaseFileAsync()
    {
        await SaveDatabaseAsync(new TaskDatabase());
    }

    /// <summary>
    /// Adds a new project plan
    /// </summary>
    /// <param name="plan">Project plan to add</param>
    public async Task AddPlanAsync(ProjectPlan plan)
    {
        var database = await GetDatabaseAsync();
        
        if (database.Plans.Any(s => s.Id == plan.Id))
        {
            throw new InvalidOperationException($"A plan with ID '{plan.Id}' already exists.");
        }

        database.Plans.Add(plan);
        await SaveDatabaseAsync(database);
    }

    /// <summary>
    /// Adds a new Task item
    /// </summary>
    /// <param name="task">Task to add</param>
    public async Task AddTaskAsync(TaskItem task)
    {
        var database = await GetDatabaseAsync();
        
        if (database.Tasks.Any(t => t.Id == task.Id))
        {
            throw new InvalidOperationException($"A task with ID '{task.Id}' already exists.");
        }

        database.Tasks.Add(task);
        await SaveDatabaseAsync(database);
    }

    /// <summary>
    /// Updates the status of a Task item
    /// </summary>
    /// <param name="taskId">ID of the Task to update</param>
    /// <param name="newStatus">New status</param>
    public async Task UpdateTaskStatusAsync(string taskId, Enums.TaskStatus newStatus)
    {
        var database = await GetDatabaseAsync();
        var task = database.Tasks.FirstOrDefault(t => t.Id == taskId);
        
        if (task == null)
        {
            throw new ArgumentException($"Task with ID '{taskId}' not found.");
        }

        task.Status = newStatus;
        task.UpdatedAt = DateTime.UtcNow;
        
        if (newStatus == Enums.TaskStatus.Completed)
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        
        await SaveDatabaseAsync(database);
    }

    /// <summary>
    /// Gets a project plan by ID with hierarchical Tasks populated
    /// </summary>
    /// <param name="planId">ID of the plan</param>
    /// <returns>Project plan with Tasks or null if not found</returns>
    public async Task<ProjectPlan?> GetPlanByIdAsync(string planId)
    {
        var database = await GetDatabaseAsync();
        var plan = database.Plans.FirstOrDefault(s => s.Id == planId);
        
        if (plan == null)
        {
            return null;
        }

        // Get all tasks for this plan
        var planTasks = database.Tasks.Where(t => t.PlanId == planId).ToList();
        
        // Build hierarchical structure and assign to plan
        plan.Tasks = BuildTaskHierarchy(planTasks);
        
        return plan;
    }

    /// <summary>
    /// Gets all project plans with hierarchical Tasks populated
    /// </summary>
    /// <returns>List of project plans with Tasks</returns>
    public async Task<List<ProjectPlan>> GetAllPlansAsync()
    {
        var database = await GetDatabaseAsync();
        
        // For each plan, populate its hierarchical tasks
        foreach (var plan in database.Plans)
        {
            var planTasks = database.Tasks.Where(t => t.PlanId == plan.Id).ToList();
            plan.Tasks = BuildTaskHierarchy(planTasks);
        }
        
        return database.Plans;
    }

    /// <summary>
    /// Gets Task items for a specific plan
    /// </summary>
    /// <param name="planId">ID of the plan</param>
    /// <returns>List of Task items for the plan</returns>
    public async Task<List<TaskItem>> GetTasksByPlanIdAsync(string planId)
    {
        var database = await GetDatabaseAsync();
        return database.Tasks.Where(t => t.PlanId == planId).ToList();
    }

    /// <summary>
    /// Gets a Task item by ID
    /// </summary>
    /// <param name="taskId">ID of the Task item</param>
    /// <returns>Task item or null if not found</returns>
    public async Task<TaskItem?> GetTaskByIdAsync(string taskId)
    {
        var database = await GetDatabaseAsync();
        return database.Tasks.FirstOrDefault(t => t.Id == taskId);
    }

    /// <summary>
    /// Builds hierarchical structure for Tasks
    /// </summary>
    /// <param name="flatTasks">Flat list of Tasks</param>
    /// <returns>List of root Tasks with children populated</returns>
    private List<TaskItem> BuildTaskHierarchy(List<TaskItem> flatTasks)
    {
        var taskDict = flatTasks.ToDictionary(t => t.Id, t => 
            new TaskItem
            {
                Id = t.Id,
                PlanId = t.PlanId,
                ParentTaskId = t.ParentTaskId,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Tags = new List<string>(t.Tags),
                Groups = new List<string>(t.Groups),
                EstimateHours = t.EstimateHours,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CompletedAt = t.CompletedAt,
                Children = new List<TaskItem>()
            });
        
        var rootTasks = new List<TaskItem>();
        
        foreach (var task in taskDict.Values)
        {
            if (string.IsNullOrEmpty(task.ParentTaskId))
            {
                rootTasks.Add(task);
            }
            else
            {
                if (taskDict.TryGetValue(task.ParentTaskId, out var parent))
                {
                    parent.Children.Add(task);
                }
                else
                {
                    rootTasks.Add(task);
                }
            }
        }
        
        return rootTasks;
    }
}
