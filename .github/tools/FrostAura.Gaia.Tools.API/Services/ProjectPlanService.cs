using FrostAura.Gaia.Tools.API.Data;
using FrostAura.Gaia.Tools.API.Models;
using Microsoft.EntityFrameworkCore;
using TaskStatus = FrostAura.Gaia.Tools.API.Models.TaskStatus;

namespace FrostAura.Gaia.Tools.API.Services;

/// <summary>
/// Service for managing project plans and tasks
/// </summary>
public class ProjectPlanService
{
    private readonly GaiaToolsDbContext _dbContext;
    private readonly ILogger<ProjectPlanService> _logger;

    public ProjectPlanService(GaiaToolsDbContext dbContext, ILogger<ProjectPlanService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates or updates a project plan with all its tasks
    /// </summary>
    /// <param name="plan">The complete project plan to upsert</param>
    /// <returns>The upserted project plan</returns>
    public async Task<ProjectPlan> UpsertPlanAsync(ProjectPlan plan)
    {
        try
        {
            if (plan == null)
                throw new ArgumentNullException(nameof(plan));

            _logger.LogInformation("Upserting plan {PlanId} - {PlanName}", plan.Id, plan.Name);

            // Check if plan exists
            var existingPlan = await _dbContext.ProjectPlans
                .Include(p => p.Tasks)
                .ThenInclude(t => t.Children)
                .ThenInclude(t => t.Children) // Support 3 levels
                .FirstOrDefaultAsync(p => p.Id == plan.Id);

            if (existingPlan != null)
            {
                // Update existing plan
                _logger.LogInformation("Updating existing plan {PlanId}", plan.Id);
                
                // Remove existing tasks to replace them
                var existingTasks = await _dbContext.TaskItems
                    .Where(t => t.PlanId == plan.Id)
                    .ToListAsync();
                _dbContext.TaskItems.RemoveRange(existingTasks);

                // Update plan properties
                existingPlan.Name = plan.Name;
                existingPlan.Description = plan.Description;
                existingPlan.AiAgentBuildContext = plan.AiAgentBuildContext;
                existingPlan.CreatorIdentity = plan.CreatorIdentity;
                existingPlan.UpdatedAt = DateTime.UtcNow;
                
                // Add new tasks
                AddTasksRecursively(plan.Tasks, plan.Id);
                
                await _dbContext.SaveChangesAsync();
                
                // Return the updated plan with tasks
                return await GetProjectPlanByIdAsync(plan.Id) ?? existingPlan;
            }
            else
            {
                // Add new plan
                _logger.LogInformation("Adding new plan {PlanId}", plan.Id);
                plan.CreatedAt = DateTime.UtcNow;
                plan.UpdatedAt = DateTime.UtcNow;
                
                _dbContext.ProjectPlans.Add(plan);
                AddTasksRecursively(plan.Tasks, plan.Id);
                
                await _dbContext.SaveChangesAsync();
                
                return plan;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert plan {PlanId}", plan?.Id ?? "unknown");
            throw;
        }
    }

    /// <summary>
    /// Recursively adds tasks to the database context
    /// </summary>
    private void AddTasksRecursively(List<TaskItem> tasks, string planId)
    {
        foreach (var task in tasks)
        {
            task.PlanId = planId;
            _dbContext.TaskItems.Add(task);
            
            if (task.Children?.Any() == true)
            {
                AddTasksRecursively(task.Children, planId);
            }
        }
    }

    /// <summary>
    /// Creates a new project plan
    /// </summary>
    /// <param name="projectName">Name of the project</param>
    /// <param name="description">Brief description of the project</param>
    /// <param name="aiAgentBuildContext">Concise context that will be needed for when the AI agent later uses the plan to build the solution</param>
    /// <param name="creatorIdentity">A best attempt at a derived user name / context, typically from the host machine details</param>
    /// <summary>
    /// Gets all project plans without their tasks (for list view)
    /// </summary>
    /// <param name="hideCompleted">Whether to hide completed plans</param>
    /// <returns>List of project plans without tasks</returns>
    public async Task<List<ProjectPlan>> ListPlansAsync(bool hideCompleted = false)
    {
        try
        {
            var query = _dbContext.ProjectPlans.AsQueryable();

            var plans = await query.ToListAsync();

            if (hideCompleted)
            {
                // We need to check completion status, so we'll need to load tasks for this check
                var plansWithTasks = await _dbContext.ProjectPlans
                    .Include(p => p.Tasks)
                    .ThenInclude(t => t.Children)
                    .ThenInclude(t => t.Children)
                    .ToListAsync();
                
                plans = plansWithTasks.Where(p => !IsProjectPlanCompleted(p))
                    .Select(p => new ProjectPlan
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        AiAgentBuildContext = p.AiAgentBuildContext,
                        CreatorIdentity = p.CreatorIdentity,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt,
                        Tasks = new List<TaskItem>() // Empty tasks list
                    }).ToList();
            }
            else
            {
                // For non-filtered results, just return plans without tasks
                foreach (var plan in plans)
                {
                    plan.Tasks = new List<TaskItem>();
                }
            }

            _logger.LogDebug("Listed {PlanCount} project plans (hideCompleted: {HideCompleted})", plans.Count, hideCompleted);
            return plans;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list project plans");
            throw;
        }
    }

    /// <summary>
    /// Gets a specific project plan by ID
    /// </summary>
    /// <param name="planId">ID of the project plan</param>
    /// <returns>The project plan with all tasks, or null if not found</returns>
    public async Task<ProjectPlan?> GetProjectPlanByIdAsync(string planId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(planId))
                throw new ArgumentException("Plan ID cannot be null or empty", nameof(planId));

            var plan = await _dbContext.ProjectPlans
                .Include(p => p.Tasks)
                .ThenInclude(t => t.Children)
                .ThenInclude(t => t.Children)
                .FirstOrDefaultAsync(p => p.Id == planId);

            if (plan != null)
            {
                _logger.LogDebug("Retrieved project plan {PlanId} with {TaskCount} root tasks", plan.Id, plan.Tasks.Count);
            }
            else
            {
                _logger.LogWarning("Project plan {PlanId} not found", planId);
            }

            return plan;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve project plan {PlanId}", planId);
            throw;
        }
    }

    /// <summary>
    /// Gets progress statistics for a project plan
    /// </summary>
    /// <param name="planId">ID of the project plan</param>
    /// <returns>Progress statistics</returns>
    public async Task<ProjectPlanProgress> GetProjectPlanProgressAsync(string planId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(planId))
                throw new ArgumentException("Plan ID cannot be null or empty", nameof(planId));

            var plan = await GetProjectPlanByIdAsync(planId);
            if (plan == null)
                throw new InvalidOperationException($"Project plan {planId} not found");

            var totalTasks = CountTotalTasks(plan.Tasks);
            var completedTasks = CountCompletedTasks(plan.Tasks);
            var inProgressTasks = CountTasksByStatus(plan.Tasks, TaskStatus.InProgress);
            var blockedTasks = CountTasksByStatus(plan.Tasks, TaskStatus.Blocked);
            var todoTasks = CountTasksByStatus(plan.Tasks, TaskStatus.Todo);

            var progress = new ProjectPlanProgress
            {
                PlanId = planId,
                PlanName = plan.Name,
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                InProgressTasks = inProgressTasks,
                BlockedTasks = blockedTasks,
                TodoTasks = todoTasks,
                TotalEstimateHours = plan.EstimateHours,
                CompletionPercentage = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0,
                IsCompleted = completedTasks == totalTasks && totalTasks > 0
            };

            _logger.LogDebug("Retrieved progress for plan {PlanId}: {CompletedTasks}/{TotalTasks} completed ({Percentage:F1}%)",
                planId, completedTasks, totalTasks, progress.CompletionPercentage);

            return progress;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get progress for project plan {PlanId}", planId);
            throw;
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Recursively counts total tasks including children
    /// </summary>
    private static int CountTotalTasks(List<TaskItem> tasks)
    {
        int count = tasks.Count;
        foreach (var task in tasks)
        {
            count += CountTotalTasks(task.Children);
        }
        return count;
    }

    /// <summary>
    /// Recursively counts completed tasks including children
    /// </summary>
    private static int CountCompletedTasks(List<TaskItem> tasks)
    {
        int count = tasks.Count(t => t.Status == TaskStatus.Completed);
        foreach (var task in tasks)
        {
            count += CountCompletedTasks(task.Children);
        }
        return count;
    }

    /// <summary>
    /// Recursively counts tasks by status including children
    /// </summary>
    private static int CountTasksByStatus(List<TaskItem> tasks, TaskStatus status)
    {
        int count = tasks.Count(t => t.Status == status);
        foreach (var task in tasks)
        {
            count += CountTasksByStatus(task.Children, status);
        }
        return count;
    }

    /// <summary>
    /// Determines if a project plan is completed (all tasks are completed)
    /// </summary>
    private static bool IsProjectPlanCompleted(ProjectPlan plan)
    {
        var totalTasks = CountTotalTasks(plan.Tasks);
        var completedTasks = CountCompletedTasks(plan.Tasks);
        return totalTasks > 0 && completedTasks == totalTasks;
    }

    #endregion
}

/// <summary>
/// Represents progress statistics for a project plan
/// </summary>
public class ProjectPlanProgress
{
    public string PlanId { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int BlockedTasks { get; set; }
    public int TodoTasks { get; set; }
    public double TotalEstimateHours { get; set; }
    public double CompletionPercentage { get; set; }
    public bool IsCompleted { get; set; }
}
