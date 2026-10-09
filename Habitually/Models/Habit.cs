// Habit.cs — Represents a single habit the user wants to track.
// Each habit has a name, emoji, color, and a weekly target (how many days per week).

using System.ComponentModel.DataAnnotations;

namespace Habitually.Models;

/// <summary>A habit owned by a user, with a weekly completion target.</summary>
public class Habit
{
    /// <summary>Unique identifier for this habit.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Identifier of the user who owns this habit.</summary>
    public Guid UserId { get; set; }

    /// <summary>User-facing name, e.g. "Morning workout".</summary>
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Single emoji shown beside the habit name.</summary>
    [MaxLength(16)]
    public string Emoji { get; set; } = "✅";

    /// <summary>Theme color from the habit picker, e.g. "var(--color-habit-amber)".</summary>
    [MaxLength(7)]
    public string Color { get; set; } = "var(--color-primary)";

    /// <summary>
    /// How many days per week the user aims to complete this habit (1–7).
    /// For example, 5 means "5× / week".
    /// </summary>
    [Range(1, 7)]
    public int WeeklyTargetDays { get; set; } = 7;

    /// <summary>UTC date and time when this habit was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Owner of this habit, when available.</summary>
    public User? User { get; set; }

    /// <summary>Completion records associated with this habit.</summary>
    public List<HabitEntry> Entries { get; set; } = new();
}
