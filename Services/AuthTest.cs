using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace eLearningApi.Services;

public class AuthTest : IAuthorizationService
{
    public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements)
    {
        var httpContext = resource as HttpContext;

        foreach (var requirement in requirements)
        {
            if (requirement is RolesAuthorizationRequirement rolesAuthReq)
            {
                var role = TokenService.GetRole(httpContext)?.ToString();
                if (rolesAuthReq.AllowedRoles.Contains(role))
                {
                    return AuthorizationResult.Success();
                }
            }
        }

        return AuthorizationResult.Failed();
    }

    public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
    {
        throw new NotImplementedException();
    }
}
