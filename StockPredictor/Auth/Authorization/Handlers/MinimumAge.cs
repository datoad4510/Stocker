using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using StockPredictor.Auth.Requirements;

namespace StockPredictor.Auth.Handlers;
public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    private readonly ILogger<MinimumAgeHandler> _logger;

    public MinimumAgeHandler(ILogger<MinimumAgeHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
    {
        _logger.LogInformation("Checking age requirement for user: {UserId}", context.User.Identity?.Name);

        var dateOfBirthClaim = context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth && c.Issuer == "http://contoso.com");

        if (dateOfBirthClaim is null)
        {
            _logger.LogWarning("No date of birth claim found for user.");
            return Task.CompletedTask;
        }

        var dateOfBirth = Convert.ToDateTime(dateOfBirthClaim.Value);
        int calculatedAge = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth > DateTime.Today.AddYears(-calculatedAge))
        {
            calculatedAge--;
        }

        if (calculatedAge >= requirement.MinimumAge)
        {
            context.Succeed(requirement);
            _logger.LogInformation("User meets age requirement.");
        }
        else
        {
            _logger.LogWarning("User does not meet the minimum age requirement.");
        }

        return Task.CompletedTask;
    }
}
