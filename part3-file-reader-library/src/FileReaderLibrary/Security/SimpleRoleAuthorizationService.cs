namespace FileReaderLibrary.Security;

/// <summary>
/// Simplistic in-memory stand-in for a real role-based security system: "admin" can read
/// anything; other roles are limited to an allow-list of file extensions. Good enough to
/// demonstrate the seam — swap this for a real implementation of
/// <see cref="IRoleAuthorizationService"/> without touching any reader/decorator code.
/// </summary>
public class SimpleRoleAuthorizationService : IRoleAuthorizationService
{
    private const string AdminRole = "admin";

    private static readonly IReadOnlyDictionary<string, HashSet<string>> AllowedExtensionsByRole =
        new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["user"] = new(StringComparer.OrdinalIgnoreCase) { ".txt" },
            ["auditor"] = new(StringComparer.OrdinalIgnoreCase) { ".txt", ".xml" },
        };

    public bool CanRead(string role, string filePath)
    {
        if (string.Equals(role, AdminRole, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!AllowedExtensionsByRole.TryGetValue(role, out var allowedExtensions))
        {
            return false;
        }

        return allowedExtensions.Contains(Path.GetExtension(filePath));
    }
}
