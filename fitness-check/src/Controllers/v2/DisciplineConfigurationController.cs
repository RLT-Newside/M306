using AutoMapper;
using FitnessCheck.Data;
using FitnessCheck.Data.DTO.Response;
using FitnessCheck.Services;
using FitnessCheck.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FitnessCheck.Controllers.v2;

/// <summary>
/// Controller for discipline-specific configuration endpoints.
/// Provides gender-specific data recorder configuration for fitness disciplines.
/// </summary>
/// <remarks>
/// Initializes a new instance of the DisciplineConfigurationController.
/// </remarks>
/// <param name="disciplineConfigurationOptions">The discipline configuration options.</param>
/// <param name="genderService">The gender service for retrieving user gender.</param>
/// <param name="mapper">The AutoMapper instance.</param>
[ApiController]
[Route("v2/discipline-configuration")]
[Authorize]
public class DisciplineConfigurationController(
    IOptions<DisciplineConfigurationOptions> disciplineConfigurationOptions,
    IGenderService genderService,
    IMapper mapper) : ControllerBase
{
    private readonly DisciplineConfigurationOptions _disciplineConfigurationOptions = disciplineConfigurationOptions.Value;
    private readonly IGenderService _genderService = genderService;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Gets the discipline configuration for a specific discipline and the authenticated user's gender.
    /// </summary>
    /// <param name="discipline">The name of the discipline (e.g., "CoreStrength", "MedicineBallPush", etc.).</param>
    /// <returns>The discipline configuration with gender-specific data recorder settings.</returns>
    /// <response code="200">Returns the discipline configuration.</response>
    /// <response code="400">Invalid discipline name provided.</response>
    /// <response code="401">Unauthorized - Missing or invalid authentication token.</response>
    [HttpGet]
    [ProducesResponseType<DisciplineConfigurationResponseDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DisciplineConfigurationResponseDTO>> GetDisciplineConfiguration(
        [FromQuery] string discipline)
    {
        if (string.IsNullOrEmpty(discipline))
        {
            return BadRequest("Discipline parameter is required.");
        }

        // Normalize discipline name to proper case for case-insensitive matching
        var normalizedDiscipline = NormalizeDisciplineName(discipline);
        if (normalizedDiscipline == null)
        {
            return BadRequest($"Invalid discipline name: {discipline}. Valid disciplines are: CoreStrength, MedicineBallPush, StandingLongJump, OneLegStand, ShuttleRun, TwelveMinutesRun.");
        }

        // Get the discipline configuration
        var disciplineConfig = _disciplineConfigurationOptions.GetConfigurationForDiscipline(normalizedDiscipline);
        if (disciplineConfig == null)
        {
            return BadRequest($"Invalid discipline name: {discipline}. Valid disciplines are: CoreStrength, MedicineBallPush, StandingLongJump, OneLegStand, ShuttleRun, TwelveMinutesRun.");
        }

        // Get user's gender
        var username = UserClaimsUtils.GetUsername(User);
        var gender = await _genderService.GetGenderAsync(username);

        // Get gender-specific configuration
        var genderSpecificConfig = disciplineConfig.GetConfigurationForGender(gender);

        // Create response DTO using AutoMapper
        var response = new DisciplineConfigurationResponseDTO
        {
            Discipline = normalizedDiscipline,
            DataRecorderConfig = _mapper.Map<DataRecorderConfigResponseDTO>(genderSpecificConfig)
        };

        return Ok(response);
    }

    /// <summary>
    /// Normalizes the discipline name to the correct case for case-insensitive matching.
    /// </summary>
    /// <param name="discipline">The discipline name to normalize.</param>
    /// <returns>The normalized discipline name or null if invalid.</returns>
    private static string? NormalizeDisciplineName(string discipline)
    {
        return discipline.ToLowerInvariant() switch
        {
            "corestrength" => "CoreStrength",
            "medicineballpush" => "MedicineBallPush",
            "standinglongjump" => "StandingLongJump",
            "onelegstand" => "OneLegStand",
            "shuttlerun" => "ShuttleRun",
            "twelveminutesrun" => "TwelveMinutesRun",
            _ => null
        };
    }
}