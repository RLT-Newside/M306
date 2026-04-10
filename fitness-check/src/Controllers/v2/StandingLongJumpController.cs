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
/// Endpoints for managing standing long jump attempts. All endpoints require JWT Bearer authentication.
/// </summary>
[ApiController]
[Route("v2/standingLongJump")]
public class StandingLongJumpController(
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
    /// Gets all standing long jump attempts for the current user in the specified school year.
    /// </summary>
    /// <param name="schoolYear">Optional school year (e.g., 2025 for 2025/26)</param>
    /// <returns>List of standing long jump attempts and related info.</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(AttemptsResponseDTO<StandingLongJumpAttemptResponseDTO, uint>), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AttemptsResponseDTO<StandingLongJumpAttemptResponseDTO, uint>>> GetStandingLongJumpAttempts([FromQuery] ushort? schoolYear = null)
        => await GetAttemptResponse<StandingLongJumpAttempt, uint, StandingLongJumpAttemptResponseDTO>(schoolYear);

    /// <summary>
    /// Adds a new standing long jump attempt for the current user.
    /// </summary>
    /// <param name="creationRequestDTO">Attempt creation data.</param>
    /// <returns>Full overview of all available standing long jump attempts for the user and discipline.</returns>
    [HttpPost]
    [Authorize]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AttemptsResponseDTO<StandingLongJumpAttemptResponseDTO, uint>), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(typeof(void), 401)]
    [ProducesResponseType(typeof(void), 500)]
    public async Task<ActionResult<AttemptsResponseDTO<StandingLongJumpAttemptResponseDTO, uint>>> AddStandingLongJump([FromBody] StandingLongJumpAttemptCreationRequestDTO creationRequestDTO)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userData = await GetUserDataAsync();
        try
        {
            await _attemptService.AddAttemptAsync<uint, StandingLongJumpAttempt, StandingLongJumpAttemptResponseDTO>(creationRequestDTO.ResultInCentimeters, userData);
            return await GetAttemptResponse<StandingLongJumpAttempt, uint, StandingLongJumpAttemptResponseDTO>();
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
    /// Deletes a standing long jump attempt by ID for the current user.
    /// </summary>
    /// <param name="id">Attempt ID.</param>
    /// <returns>No content if deleted, NotFound if not found.</returns>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> DeleteStandingLongJumpAttempts(Guid id)
    {
        var userId = Utils.UserClaimsUtils.GetUserId(User);
        var success = await _attemptService.DeleteAttemptAsync<StandingLongJumpAttempt, uint>(id, userId);
        if (!success) return NotFound();
        return NoContent();
    }
}
