using eLearningApi.DataAccess;
using eLearningApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace eLearningApi.UserRolesAndPermissions.Attributes;

public class AccessOwnResourceOnlyAttribute : AuthorizationHandler<OwnerAuthorizationRequirement>
{
    private readonly ISubjectRepository _subjectRepository;

    public AccessOwnResourceOnlyAttribute(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerAuthorizationRequirement requirement)
    {
        var httpContext = context.Resource as HttpContext;
        // Get the user ID from the request path or query string
        string? requesterId = TokenService.GetId(httpContext);
        string? requestedId = httpContext.Request.Query["id"].ToString();

        // Check if the user is authorized to view the profile
        if (!IsAuthorized(requesterId, requestedId))
        {
            context.Fail(new AuthorizationFailureReason(this, "The user is not authorized to access this resource."));
        }

        return Task.CompletedTask;
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