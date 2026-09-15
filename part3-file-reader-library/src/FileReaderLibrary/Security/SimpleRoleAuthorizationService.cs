namespace FileReaderLibrary.Security;

// "admin" reads anything; other roles are limited to an allow-list of file extensions.
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
