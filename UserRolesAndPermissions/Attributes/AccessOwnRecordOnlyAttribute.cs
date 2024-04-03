using eLearningApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eLearningApi.UserRolesAndPermissions.Attributes;

/// <summary>
/// Specifies that the requester can only access its own records.
/// </summary>
public class AccessOwnRecordOnlyAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly IEnumerable<Role> _roles;
    public AccessOwnRecordOnlyAttribute(params Role[] roles)
    {
        if (roles.Any())
        {
            _roles = roles;
        }
        else
        {
            _roles = [.. Enum.GetValues(typeof(Role)).Cast<Role>()];
        }
    }

    public virtual Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Check if the requestor is candidate for checking
        if (!IsCandidate(context))
        {
            return Task.CompletedTask;
        }

        // Get the user ID from the request path or query string
        string? requesterId = TokenService.GetId(context.HttpContext);
        string? requestedId = context.HttpContext.Request.RouteValues["id"]?.ToString();

        // Check if the user is authorized to view the profile
        if (!IsAuthorized(requesterId, requestedId))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        return Task.CompletedTask;
    }

    protected bool IsCandidate(AuthorizationFilterContext context)
    {
        // Check if the requestor is candidate for checking
        Role? role = TokenService.GetRole(context.HttpContext);
        if (role is Role _role)
        {
            return _roles.Contains(_role);
        }

        return false;
    }

    protected bool IsAuthorized(string? requesterId, string? targetUserId)
    {
        if (string.IsNullOrEmpty(requesterId))
        {
            return false;
        }

        return requesterId == targetUserId;
    }
}
