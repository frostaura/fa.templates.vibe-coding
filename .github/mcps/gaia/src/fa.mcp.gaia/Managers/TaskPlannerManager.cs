using FrostAura.MCP.Gaia.Interfaces;
using FrostAura.MCP.Gaia.Models;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace FrostAura.MCP.Gaia.Managers;

/// <summary>
/// Manager for Task planning operations with integrated MCP tools
/// </summary>
[McpServerToolType]
public class TaskPlannerManager : ITaskPlannerManager
{
    private readonly ITaskPlannerRepository _repository;

    /// <summary>
    /// Initializes a new instance of the TaskPlannerManager
    /// </summary>
    /// <param name="repository">Task repository</param>
    /// <param name="configuration">Configuration to check for error handling preferences</param>
    public TaskPlannerManager(ITaskPlannerRepository repository, IConfiguration configuration)
    {
        _repository = repository;
    }

    /// <summary>
    /// Creates a new project plan via MCP
    /// </summary>
    /// <param name="projectName">Name of the project</param>
    /// <param name="description">Brief description that an AI can understand</param>
    /// <param name="aiAgentBuildContext">Concise context that will be needed for when the AI agent later uses the plan to build the solution</param>
    /// <param name="estimateHours">Estimated total hours for completing the project</param>
    /// <returns>JSON string containing the created project plan</returns>
    [McpServerTool]
    [Description("Creates a new project plan for managing Tasks & TODOs. Ideal for tracking tasks and features of complex projects and plans. The response is your Task plan id, which you must use to manage your Tasks.")]
    public async Task<string> NewPlanAsync(
        [Description("Name of the project")] string projectName,
        [Description("Brief description of the project")] string description,
        [Description("Concise context that will be needed for when the AI agent later uses the plan to build the solution")] string aiAgentBuildContext,
        [Description("Estimated total hours for completing the project")] double estimateHours)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(projectName))
            throw new ArgumentException("Project name cannot be null or empty.", nameof(projectName));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty.", nameof(description));
        if (string.IsNullOrWhiteSpace(aiAgentBuildContext))
            throw new ArgumentException("AI agent build context cannot be null or empty.", nameof(aiAgentBuildContext));
        if (estimateHours < 0)
            throw new ArgumentException("Estimate hours cannot be negative.", nameof(estimateHours));

        var plan = new ProjectPlan
        {
            Id = Guid.NewGuid().ToString(),
            Name = projectName,
            Description = description,
            AiAgentBuildContext = aiAgentBuildContext,
            EstimateHours = estimateHours,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        await _repository.AddPlanAsync(plan);
        var json = JsonSerializer.Serialize(plan);
        return json;
    }

    /// <summary>
    /// Lists all project plans via MCP
    /// </summary>
    /// <param name="hideCompleted">Optional string to hide completed plans ("true" to hide, anything else to show all)</param>
    /// <returns>JSON string containing all project plans with their status and progress information</returns>
    [McpServerTool]
    [Description("Lists all project plans with their IDs, names, descriptions, progress metrics, and calculated status. Does not include tasks.")]
    public async Task<string> ListPlansAsync(
        [Description("Optional string to hide completed plans (\"true\" to hide completed plans, anything else shows all)")] string? hideCompleted = null)
    {
        // Parse hideCompleted parameter
        bool shouldHideCompleted = !string.IsNullOrEmpty(hideCompleted) && 
                                   hideCompleted.Equals("true", StringComparison.OrdinalIgnoreCase);

        // Get all plans
        var plans = await _repository.GetAllPlansAsync();
        
        // Create plan summaries with calculated status and progress
        var planSummaries = new List<object>();
        
        foreach (var plan in plans)
        {
            // Get tasks for this plan to calculate status and progress
            var tasks = await _repository.GetTasksByPlanAsync(plan.Id);
            
            // Calculate plan status and progress metrics
            string planStatus = "Todo"; // Default status
            
            // Calculate progress statistics
            var totalTasks = tasks.Count;
            var completedTasks = tasks.Count(t => t.Status == Enums.TaskStatus.Completed);
            var inProgressTasks = tasks.Count(t => t.Status == Enums.TaskStatus.InProgress);
            var pendingTasks = tasks.Count(t => t.Status == Enums.TaskStatus.Todo);
            
            if (tasks.Any())
            {
                if (completedTasks == totalTasks)
                {
                    planStatus = "Completed";
                }
                else if (inProgressTasks > 0 || completedTasks > 0)
                {
                    planStatus = "InProgress";
                }
                // else remains "Todo"
            }
            
            // Skip completed plans if hideCompleted is true
            if (shouldHideCompleted && planStatus == "Completed")
            {
                continue;
            }
            
            // Calculate estimate hours by status
            var totalTaskEstimateHours = tasks.Sum(t => t.EstimateHours);
            var completedTaskEstimateHours = tasks.Where(t => t.Status == Enums.TaskStatus.Completed).Sum(t => t.EstimateHours);
            var inProgressTaskEstimateHours = tasks.Where(t => t.Status == Enums.TaskStatus.InProgress).Sum(t => t.EstimateHours);
            var pendingTaskEstimateHours = tasks.Where(t => t.Status == Enums.TaskStatus.Todo).Sum(t => t.EstimateHours);
            
            // Calculate completion percentages
            var completionPercentage = totalTasks > 0 ? Math.Round((double)completedTasks / totalTasks * 100, 2) : 0.0;
            var estimateCompletionPercentage = totalTaskEstimateHours > 0 ? Math.Round(completedTaskEstimateHours / totalTaskEstimateHours * 100, 2) : 0.0;
            
            planSummaries.Add(new
            {
                id = plan.Id,
                name = plan.Name,
                description = plan.Description,
                aiAgentBuildContext = plan.AiAgentBuildContext,
                estimateHours = plan.EstimateHours,
                status = planStatus,
                progress = new
                {
                    totalTasks,
                    completedTasks,
                    inProgressTasks,
                    pendingTasks,
                    completionPercentage,
                    totalTaskEstimateHours,
                    completedTaskEstimateHours,
                    inProgressTaskEstimateHours,
                    pendingTaskEstimateHours,
                    estimateCompletionPercentage
                },
                createdAt = plan.CreatedAt,
                updatedAt = plan.UpdatedAt
            });
        }
        
        var json = JsonSerializer.Serialize(planSummaries);
        return json;
    }

    /// <summary>
    /// Gets all tasks from a specific plan via MCP
    /// </summary>
    /// <param name="planId">ID of the plan to get tasks from</param>
    /// <param name="hideCompleted">Optional string to hide completed tasks ("true" to hide, anything else to show all)</param>
    /// <returns>JSON string containing all tasks for the specified plan</returns>
    [McpServerTool]
    [Description("Gets all tasks from a specific plan with their IDs, titles, descriptions, and status details.")]
    public async Task<string> GetTasksFromPlan(
        [Description("ID of the plan to get tasks from")] string planId,
        [Description("Optional string to hide completed tasks (\"true\" to hide completed tasks, anything else shows all)")] string? hideCompleted = null)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(planId))
            throw new ArgumentException("Plan ID cannot be null or empty.", nameof(planId));

        // Parse hideCompleted parameter
        bool shouldHideCompleted = !string.IsNullOrEmpty(hideCompleted) && 
                                   hideCompleted.Equals("true", StringComparison.OrdinalIgnoreCase);

        // Get all tasks for the plan
        var tasks = await _repository.GetTasksByPlanAsync(planId);
        
        // Filter out completed tasks if requested
        if (shouldHideCompleted)
        {
            tasks = tasks.Where(t => t.Status != Enums.TaskStatus.Completed).ToList();
        }
        
        // Create task summaries
        var taskSummaries = tasks.Select(task => new
        {
            id = task.Id,
            planId = task.PlanId,
            title = task.Title,
            description = task.Description,
            tags = task.Tags,
            groups = task.Groups,
            parentTaskId = task.ParentTaskId,
            status = task.Status.ToString(),
            estimateHours = task.EstimateHours,
            createdAt = task.CreatedAt,
            updatedAt = task.UpdatedAt
        }).ToList();
        
        var json = JsonSerializer.Serialize(taskSummaries);
        return json;
    }

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
    [McpServerTool]
    [Description("Adds a new Task / TODO item to a project plan. 3-levels deep nesting of tasks to compartmentalize complex tasks is recommended for plans.")]
    public async Task<string> AddTaskToPlanAsync(
        [Description("ID of the project plan, as from the create new plan response.")] string planId,
        [Description("Title/description of the Task / TODO that an AI can understand")] string title,
        [Description("Detailed description with acceptance criteria, important references like docs, rules, restrictions, file & directory paths")] string description,
        [Description("Comma-separated tags for categorizing Tasks. Like dev, test, analysis etc")] string tags,
        [Description("Comma-separated groups for organizing Tasks (e.g., releases, components)")] string groups,
        [Description("ID of parent Task if this is a child of another Task")] string? parentTaskId,
        [Description("Estimated hours for completing this Task")] double estimateHours)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(planId))
            throw new ArgumentException("Plan ID cannot be null or empty.", nameof(planId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));
        if (description == null)
            throw new ArgumentNullException(nameof(description));
        if (tags == null)
            throw new ArgumentNullException(nameof(tags));
        if (groups == null)
            throw new ArgumentNullException(nameof(groups));
        if (estimateHours < 0)
            throw new ArgumentException("Estimate hours cannot be negative.", nameof(estimateHours));

        var tagList = string.IsNullOrWhiteSpace(tags) ? new List<string>() : tags.Split(',').Select(t => t.Trim()).ToList();
        var groupList = string.IsNullOrWhiteSpace(groups) ? new List<string>() : groups.Split(',').Select(g => g.Trim()).ToList();
        
        var task = new TaskItem
        {
            Id = Guid.NewGuid().ToString(),
            PlanId = planId,
            Title = title,
            Description = description,
            Tags = tagList,
            Groups = groupList,
            ParentTaskId = parentTaskId,
            Status = Enums.TaskStatus.Todo,
            EstimateHours = estimateHours,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        await _repository.AddTaskAsync(task);
        var json = JsonSerializer.Serialize(task);
        return json;
    }

    /// <summary>
    /// Complete the next outstanding leaf node task and return the context of the next outstanding task via MCP
    /// </summary>
    /// <param name="planId">ID of the plan</param>
    /// <returns>JSON string containing the completed task and next task context</returns>
    [McpServerTool]
    [Description("Complete the very next outstanding leaf node task and return the context of the next outstanding task. This tool takes in a plan id arg, completes the next leaf task, and returns the next task to work on. Tl;dr complete the first incomplete task and return the one after that.")]
    public async Task<string> NextTaskFromPlanAsync(
        [Description("ID of the plan to process the next task for, as from the start plan response.")] string planId)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(planId))
            throw new ArgumentException("Plan ID cannot be null or empty.", nameof(planId));

        // Get all tasks for the plan
        var tasks = await _repository.GetTasksByPlanAsync(planId);
        
        if (!tasks.Any())
        {
            var noTasksResult = new { 
                message = "No tasks found in plan", 
                planId, 
                completedTask = (TaskItem?)null, 
                nextTask = (TaskItem?)null 
            };
            return JsonSerializer.Serialize(noTasksResult);
        }

        // Find the next outstanding leaf node task (task with no children that is not completed)
        var leafTasks = tasks.Where(t => !tasks.Any(child => child.ParentTaskId == t.Id)).ToList();
        var outstandingLeafTask = leafTasks
            .Where(t => t.Status != Enums.TaskStatus.Completed && t.Status != Enums.TaskStatus.Cancelled)
            .OrderBy(t => t.CreatedAt)
            .FirstOrDefault();

        if (outstandingLeafTask == null)
        {
            var allCompletedResult = new { 
                message = "All leaf tasks are completed", 
                planId, 
                completedTask = (TaskItem?)null, 
                nextTask = (TaskItem?)null 
            };
            return JsonSerializer.Serialize(allCompletedResult);
        }

        // Complete the outstanding leaf task
        await _repository.UpdateTaskStatusAsync(outstandingLeafTask.Id, Enums.TaskStatus.Completed);
        
        // Get the updated task
        var completedTask = await _repository.GetTaskByIdAsync(outstandingLeafTask.Id);

        // Find the next outstanding task
        var remainingTasks = await _repository.GetTasksByPlanAsync(planId);
        var remainingLeafTasks = remainingTasks.Where(t => !remainingTasks.Any(child => child.ParentTaskId == t.Id)).ToList();
        var nextTask = remainingLeafTasks
            .Where(t => t.Status != Enums.TaskStatus.Completed && t.Status != Enums.TaskStatus.Cancelled)
            .OrderBy(t => t.CreatedAt)
            .FirstOrDefault();

        var result = new { 
            message = "Task completed successfully", 
            planId, 
            completedTask, 
            nextTask 
        };
        
        return JsonSerializer.Serialize(result);
    }
}
