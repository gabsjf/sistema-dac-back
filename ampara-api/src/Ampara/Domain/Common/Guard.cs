using System.Text.RegularExpressions;

namespace Ampara.Domain.Common;

// Small validation helpers shared by the entities. Messages are user-facing (pt-BR).
public static partial class Guard
{
    public static string Required(string? value, string fieldLabel, int maxLength)
    {
        var trimmed = value?.Trim() ?? string.Empty;

        if (trimmed.Length == 0)
            throw new DomainException($"O campo {fieldLabel} é obrigatório.");

        return MaxLength(trimmed, fieldLabel, maxLength);
    }

    public static string? Optional(string? value, string fieldLabel, int maxLength)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : MaxLength(trimmed, fieldLabel, maxLength);
    }

    public static string RequiredEmail(string? value, string fieldLabel, int maxLength)
    {
        var email = Required(value, fieldLabel, maxLength);

        if (!EmailPattern().IsMatch(email))
            throw new DomainException($"O campo {fieldLabel} não é um e-mail válido.");

        return email;
    }

    private static string MaxLength(string value, string fieldLabel, int maxLength)
    {
        if (value.Length > maxLength)
            throw new DomainException($"O campo {fieldLabel} deve ter no máximo {maxLength} caracteres.");

        return value;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailPattern();
}
