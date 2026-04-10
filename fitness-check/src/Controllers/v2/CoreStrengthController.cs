using FitnessCheck.Data;
using FitnessCheck.Data.DTO.Request;
using FitnessCheck.Data.DTO.Response;
using FitnessCheck.Data.Entities;
using FitnessCheck.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FitnessCheck.Controllers.v2;

/// <summary>
/// Endpoints for managing core strength attempts. All endpoints require JWT Bearer authentication.
/// </summary>
[ApiController]
[Route("v2/coreStrength")]
public class CoreStrengthController(
    FitnessCheckDbContext dbContext,
    AutoMapper.IMapper mapper,
    IGenderService genderService,
    ICohortService cohortService,
    IBestAttemptService bestAttemptService,
    IAttemptService attemptService,
    IOptions<MaxAllowedAttemptsOptions> maxAllowedAttempts) : AttemptController(dbContext,
           mapper,
           genderService,
           cohortService,
           bestAttemptService,
           attemptService,
           maxAllowedAttempts)
{

    /// <summary>
    /// Gets all core strength attempts for the current user in the specified school year.
    /// </summary>
    /// <param name="schoolYear">Optional school year (e.g., 2025 for 2025/26)</param>
    /// <returns>List of core strength attempts and related info.</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(AttemptsResponseDTO<CoreStrengthAttemptResponseDTO, uint>), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AttemptsResponseDTO<CoreStrengthAttemptResponseDTO, uint>>> GetCoreStrengthAttempts([FromQuery] ushort? schoolYear = null)
        => await GetAttemptResponse<CoreStrengthAttempt, uint, CoreStrengthAttemptResponseDTO>(schoolYear);

    /// <summary>
    /// Adds a new core strength attempt for the current user.
    /// </summary>
    /// <param name="creationRequestDTO">Attempt creation data.</param>
    /// <returns>The created attempt.</returns>
    [HttpPost]
    [Authorize]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AttemptsResponseDTO<CoreStrengthAttemptResponseDTO, uint>), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(typeof(void), 401)]
    [ProducesResponseType(typeof(void), 500)]
    public async Task<ActionResult<AttemptsResponseDTO<CoreStrengthAttemptResponseDTO, uint>>> AddCoreStrength([FromBody] CoreStrengthAttemptCreationRequestDTO creationRequestDTO)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userData = await GetUserDataAsync();
        try
        {
            await _attemptService.AddAttemptAsync<uint, CoreStrengthAttempt, CoreStrengthAttemptResponseDTO>(creationRequestDTO.ResultInSeconds, userData);
            return await GetAttemptResponse<CoreStrengthAttempt, uint, CoreStrengthAttemptResponseDTO>();
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return BadRequest(exception.Message);
        }
        catch
        {
            return StatusCode(500, "Failed to add attempt.");
        }
    }

    /// <summary>
    /// Deletes a core strength attempt by ID for the current user.
    /// </summary>
    /// <param name="id">Attempt ID.</param>
    /// <returns>No content if deleted, NotFound if not found.</returns>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> DeleteCoreStrengthAttempts(Guid id)
    {
        var userId = Utils.UserClaimsUtils.GetUserId(User);
        var success = await _attemptService.DeleteAttemptAsync<CoreStrengthAttempt, uint>(id, userId);
        if (!success) return NotFound();
        return NoContent();
    }
}
