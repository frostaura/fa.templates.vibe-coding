using FrostAura.Gaia.Tools.API.Models;
using FrostAura.Gaia.Tools.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrostAura.Gaia.Tools.API.Controllers;

/// <summary>
/// Controller for managing project plans through the ProjectPlanService
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PlansController : ControllerBase
{
    private readonly ProjectPlanService _projectPlanService;
    private readonly ILogger<PlansController> _logger;

    public PlansController(ProjectPlanService projectPlanService, ILogger<PlansController> logger)
    {
        _projectPlanService = projectPlanService ?? throw new ArgumentNullException(nameof(projectPlanService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates or updates a project plan with all its tasks
    /// </summary>
    /// <param name="plan">The complete project plan to upsert</param>
    /// <returns>The upserted project plan</returns>
    [HttpPost]
    public async Task<IActionResult> UpsertPlan([FromBody] ProjectPlan plan)
    {
        try
        {
            if (plan == null)
                return BadRequest("Plan data is required");

            var upsertedPlan = await _projectPlanService.UpsertPlanAsync(plan);
            return Ok(upsertedPlan);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid plan data provided");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert project plan {PlanId}", plan?.Id ?? "unknown");
            return StatusCode(500, "Failed to upsert project plan");
        }
    }

    /// <summary>
    /// Gets all project plans without their tasks (for list view)
    /// </summary>
    /// <param name="hideCompleted">Whether to hide completed plans</param>
    /// <returns>List of project plans without tasks</returns>
    [HttpGet]
    public async Task<IActionResult> ListPlans([FromQuery] bool hideCompleted = false)
    {
        try
        {
            var plans = await _projectPlanService.ListPlansAsync(hideCompleted);
            return Ok(plans);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list project plans");
            return StatusCode(500, "Failed to list project plans");
        }
    }

    /// <summary>
    /// Gets a specific project plan by ID with all tasks
    /// </summary>
    /// <param name="id">Project plan ID</param>
    /// <returns>Project plan with all tasks</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlan(string id)
    {
        try
        {
            var plan = await _projectPlanService.GetProjectPlanByIdAsync(id);
            if (plan == null)
                return NotFound($"Project plan {id} not found");

            return Ok(plan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve project plan {PlanId}", id);
            return StatusCode(500, "Failed to retrieve project plan");
        }
    }

    /// <summary>
    /// Gets progress statistics for a project plan
    /// </summary>
    /// <param name="id">Project plan ID</param>
    /// <returns>Progress statistics</returns>
    [HttpGet("{id}/progress")]
    public async Task<IActionResult> GetPlanProgress(string id)
    {
        try
        {
            var progress = await _projectPlanService.GetProjectPlanProgressAsync(id);
            return Ok(progress);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Project plan {PlanId} not found", id);
            return NotFound($"Project plan {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve progress for project plan {PlanId}", id);
            return StatusCode(500, "Failed to retrieve project plan progress");
        }
    }
}
