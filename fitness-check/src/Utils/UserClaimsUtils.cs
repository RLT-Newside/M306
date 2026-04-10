using System.Security.Claims;
using System.Text.RegularExpressions;

namespace FitnessCheck.Utils;

/// <summary>
/// Utility methods for extracting user information from claims.
/// </summary>
public static class UserClaimsUtils
{
    /// <summary>
    /// Extracts the user ID (Guid) from the claims principal.
    /// </summary>
    /// <param name="user">The claims principal.</param>
    /// <returns>The user ID as a Guid.</returns>
    /// <exception cref="ArgumentException">Thrown if the claim is missing or invalid.</exception>
    public static Guid GetUserId(ClaimsPrincipal user)
    {
        string? userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new ArgumentException($"No claim of type '{ClaimTypes.NameIdentifier}' was found.", ClaimTypes.NameIdentifier);
        }
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            throw new ArgumentException($"Value for claim '{ClaimTypes.NameIdentifier}' ({userIdString}) cannot be parsed as Guid.", ClaimTypes.NameIdentifier);
        }
        return userId;
    }

    /// <summary>
    /// Extracts the username from the claims principal (using UPN claim).
    /// </summary>
    /// <param name="user">The claims principal.</param>
    /// <returns>The username as a string.</returns>
    /// <exception cref="ArgumentException">Thrown if the claim is missing or invalid.</exception>
    public static string GetUsername(ClaimsPrincipal user)
    {
        string? usernameString = user.FindFirstValue(ClaimTypes.Upn);
        if (string.IsNullOrEmpty(usernameString))
        {
            throw new ArgumentException($"No claim of type '{ClaimTypes.Upn}' was found.", ClaimTypes.Upn);
        }
        var upnPattern = @"^([A-z]{2,}\d{0,2})(?=@?)";
        var upnRegex = new Regex(upnPattern);
        var matches = upnRegex.Matches(usernameString);
        if (matches.Count == 0)
        {
            throw new ArgumentException($"Invalid value for claim of type '{ClaimTypes.Upn}': '{usernameString}'.", ClaimTypes.Upn);
        }
        var username = matches[0].Value;
        return username;
    }
}
