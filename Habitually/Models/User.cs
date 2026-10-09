using System.ComponentModel.DataAnnotations;

namespace Habitually.Models;

/// <summary>A user who owns habits and personal app settings.</summary>
public class User
{
    /// <summary>Unique identifier for this user.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Name shown for the user in the app.</summary>
    [Required]
    [MaxLength(50)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>UTC date and time when this user was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Habits owned by this user.</summary>
    public List<Habit> Habits { get; set; } = new();

    /// <summary>Personal app settings for this user, when available.</summary>
    public UserSettings? Settings { get; set; }
}
