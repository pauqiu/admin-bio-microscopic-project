namespace UCR.EB.BioMicroscopeAdmin.Backend.Application.Security;

/// <summary>Roles available in the platform.</summary>
public static class AuthRoles
{
    public const string Administrator = "Administrador";
    public const string Reader = "Lector";

    public static readonly IReadOnlyList<string> All = [Administrator, Reader];
}

/// <summary>Authorization policy names.</summary>
public static class AuthPolicies
{
    /// <summary>Any user with an active session and a valid role.</summary>
    public const string Reader = "RequireReader";

    /// <summary>Only users with the administrator role.</summary>
    public const string Administrator = "RequireAdministrator";
}
