namespace eLearningApi.UserRolesAndPermissions.Attributes;

public class PermittedAttribute : Attribute
{
    /// <summary>
    /// Permitted roles
    /// </summary>
    public readonly IEnumerable<Role> Roles;

    public PermittedAttribute(params Role[] roles)
    {
        Roles = roles;
    }
}
