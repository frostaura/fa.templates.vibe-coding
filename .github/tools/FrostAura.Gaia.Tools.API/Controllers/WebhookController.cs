using FrostAura.Gaia.Tools.API.Data;
using FrostAura.Gaia.Tools.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrostAura.Gaia.Tools.API.Controllers;

/// <summary>
/// Controller for receiving webhook notifications from the Gaia MCP server
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly ILogger<WebhookController> _logger;
    private readonly GaiaToolsDbContext _dbContext;

    public WebhookController(ILogger<WebhookController> logger, GaiaToolsDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Receives project plan webhook notifications
    /// </summary>
    /// <param name="plan">The project plan data from the webhook</param>
    /// <returns>OK response if successful</returns>
    [HttpPost]
    public async Task<IActionResult> ReceivePlanWebhook([FromBody] ProjectPlan plan)
    {
        try
        {
            if (plan == null)
            {
                _logger.LogWarning("Received webhook with null plan data");
                return BadRequest("Plan data is required");
            }

            _logger.LogInformation("Received webhook for plan {PlanId} - {PlanName}", plan.Id, plan.Name);
            _logger.LogDebug("Plan details: {TaskCount} tasks, {EstimateHours} hours estimated", 
                plan.Tasks.Count, plan.EstimateHours);

            // Process the plan data
            await ProcessPlanAsync(plan);

            return Ok(new { message = "Webhook received successfully", planId = plan.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook for plan {PlanId}", plan?.Id ?? "unknown");
            return StatusCode(500, "Internal server error processing webhook");
        }
    }

    /// <summary>
    /// Health check endpoint for webhook availability
    /// </summary>
    /// <returns>OK response if webhook endpoint is available</returns>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Processes the received plan data and stores it in the database
    /// </summary>
    /// <param name="plan">The project plan to process</param>
    private async Task ProcessPlanAsync(ProjectPlan plan)
    {
        // Log some basic statistics about the plan
        var totalTasks = CountTotalTasks(plan.Tasks);
        var completedTasks = CountCompletedTasks(plan.Tasks);
        var progress = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0;

        _logger.LogInformation("Plan {PlanId} progress: {CompletedTasks}/{TotalTasks} tasks completed ({Progress:F1}%)",
            plan.Id, completedTasks, totalTasks, progress);

        // Log nested task structure
        LogTaskHierarchy(plan.Tasks, 0);

        try
        {
            // Store or update the plan in the database
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
                existingPlan.UpdatedAt = DateTime.UtcNow;
                
                // Add new tasks
                await AddTasksRecursively(plan.Tasks);
            }
            else
            {
                // Add new plan
                _logger.LogInformation("Adding new plan {PlanId}", plan.Id);
                plan.CreatedAt = DateTime.UtcNow;
                plan.UpdatedAt = DateTime.UtcNow;
                
                _dbContext.ProjectPlans.Add(plan);
                await AddTasksRecursively(plan.Tasks);
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Successfully saved plan {PlanId} to database", plan.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save plan {PlanId} to database", plan.Id);
            // Don't rethrow - webhook should still return success even if DB fails
        }
    }

    /// <summary>
    /// Recursively adds tasks to the database context
    /// </summary>
    private async Task AddTasksRecursively(List<TaskItem> tasks)
    {
        foreach (var task in tasks)
        {
            _dbContext.TaskItems.Add(task);
            if (task.Children.Any())
            {
                await AddTasksRecursively(task.Children);
            }
        }
    }

    /// <summary>
    /// Logs the hierarchical structure of tasks
    /// </summary>
    private void LogTaskHierarchy(List<TaskItem> tasks, int level)
    {
        var indent = new string(' ', level * 2);
        foreach (var task in tasks)
        {
            _logger.LogDebug("{Indent}Level {Level}: {TaskTitle} (Status: {Status}, Hours: {Hours})",
                indent, level, task.Title, task.Status, task.EstimateHours);
            
            if (task.Children.Any())
            {
                LogTaskHierarchy(task.Children, level + 1);
            }
        }
    }

    /// <summary>
    /// Recursively counts total tasks including children
    /// </summary>
    private int CountTotalTasks(List<TaskItem> tasks)
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
    private int CountCompletedTasks(List<TaskItem> tasks)
    {
        int count = tasks.Count(t => t.Status == Models.TaskStatus.Completed);
        foreach (var task in tasks)
        {
            count += CountCompletedTasks(task.Children);
        }
        return count;
    }
}
