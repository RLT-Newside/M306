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
/// Endpoints for managing medicine ball push attempts. All endpoints require JWT Bearer authentication.
/// </summary>
[ApiController]
[Route("v2/medicineBallPush")]
public class MedicineBallPushController(
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
    /// Gets all medicine ball push attempts for the current user in the specified school year.
    /// </summary>
    /// <param name="schoolYear">Optional school year (e.g., 2025 for 2025/26)</param>
    /// <returns>List of medicine ball push attempts and related info.</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(AttemptsResponseDTO<MedicineBallPushAttemptResponseDTO, uint>), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AttemptsResponseDTO<MedicineBallPushAttemptResponseDTO, uint>>> GetMedicineBallPushAttempts([FromQuery] ushort? schoolYear = null)
        => await GetAttemptResponse<MedicineBallPushAttempt, uint, MedicineBallPushAttemptResponseDTO>(schoolYear);

    /// <summary>
    /// Adds a new medicine ball push attempt for the current user.
    /// </summary>
    /// <param name="creationRequestDTO">Attempt creation data.</param>
    /// <returns>The created attempt.</returns>
    [HttpPost]
    [Authorize]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AttemptsResponseDTO<MedicineBallPushAttemptResponseDTO, uint>), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(typeof(void), 401)]
    [ProducesResponseType(typeof(void), 500)]
    public async Task<ActionResult<AttemptsResponseDTO<MedicineBallPushAttemptResponseDTO, uint>>> AddMedicineBallPush([FromBody] MedicineBallPushAttemptCreationRequestDTO creationRequestDTO)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userData = await GetUserDataAsync();
        try
        {
            await _attemptService.AddAttemptAsync<uint, MedicineBallPushAttempt, MedicineBallPushAttemptResponseDTO>(creationRequestDTO.ResultInCentimeters, userData);
            return await GetAttemptResponse<MedicineBallPushAttempt, uint, MedicineBallPushAttemptResponseDTO>();
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
    /// Deletes a medicine ball push attempt by ID for the current user.
    /// </summary>
    /// <param name="id">Attempt ID.</param>
    /// <returns>No content if deleted, NotFound if not found.</returns>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> DeleteMedicineBallPushAttempts(Guid id)
    {
        var userId = Utils.UserClaimsUtils.GetUserId(User);
        var success = await _attemptService.DeleteAttemptAsync<MedicineBallPushAttempt, uint>(id, userId);
        if (!success) return NotFound();
        return NoContent();
    }
}
