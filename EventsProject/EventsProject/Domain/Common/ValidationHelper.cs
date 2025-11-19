using System.Text;
using System.Text.RegularExpressions;

namespace EventsProject.Domain.Common;

//Clase para validaciones genericas y especificas de acuerdo a la logica de negocio definida
public static class ValidationHelper {
    public static void ValidateLength(string? value, int min, int max, string fieldName, StringBuilder errorsBuilder) {
        if (string.IsNullOrWhiteSpace(value) || value.Length < min || value.Length > max)
            errorsBuilder.AppendLine($"- {fieldName} must have between {min} and {max} characters");
    }

    public static void ValidateRange(int? value, int min, int max, string fieldName, StringBuilder errorsBuilder) {
        if (value == null || value < min || value > max)
            errorsBuilder.AppendLine($"- {fieldName} must be between {min} and {max}");
    }
    
    public static void ValidateNull(object? value, string fieldName, StringBuilder errorsBuilder) {
        if (value == null)
            errorsBuilder.AppendLine($"- {fieldName} is required");
    }
    
    public static void ValidateDates(DateTime? initial, DateTime? end, StringBuilder errorsBuilder) {
        if (initial == null || end == null)
            errorsBuilder.AppendLine("- Start and End date/time must be selected");
        else if (initial >= end)
            errorsBuilder.AppendLine("- EndDateTime must be later than InitialDateTime");
    }
    
    public static void ValidateEmail(string email, StringBuilder errorsBuilder) {
        if (!Regex.IsMatch(email, ValidationConsts.EmailPattern, RegexOptions.IgnoreCase))
            errorsBuilder.AppendLine("- Email address pattern invalid");
    }
    
    public static void ValidatePassword(string password, StringBuilder errorsBuilder) {
        if (!Regex.IsMatch(password, ValidationConsts.PasswordPattern, RegexOptions.IgnoreCase))
            errorsBuilder.AppendLine($"- Password pattern invalid, it must have between " +
                $"{ValidationConsts.MinPasswordLength} and {ValidationConsts.MaxPasswordLength} characters, " +
                "at least one digit, one uppercase and lowercase letter");
    }
}
