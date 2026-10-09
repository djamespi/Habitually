using System.ComponentModel.DataAnnotations;

namespace Habitually.Models;

/// <summary>Stores a user's preferred app appearance.</summary>
public class UserSettings
{
    /// <summary>Unique identifier for these settings.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Identifier of the user who owns these settings.</summary>
    public Guid UserId { get; set; }

    /// <summary>Preferred appearance mode, initially light.</summary>
    public ThemeMode Theme { get; set; } = ThemeMode.Light;

    /// <summary>Preferred accent color as a hexadecimal color value.</summary>
    [MaxLength(7)]
    public string AccentColor { get; set; } = "#5b4be1";

    /// <summary>Owner of these settings, when available.</summary>
    public User? User { get; set; }
}
