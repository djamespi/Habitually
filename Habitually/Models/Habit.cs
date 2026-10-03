// Habit.cs — Represents a single habit the user wants to track.
// Each habit has a name, emoji, color, and a weekly target (how many days per week).

namespace Habitually.Models;

public class Habit
{
    /// <summary>Unique identifier for this habit.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>User-facing name, e.g. "Morning workout".</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Single emoji shown beside the habit name.</summary>
    public string Emoji { get; set; } = "✅";

    /// <summary>Theme color from the habit picker, e.g. "var(--color-habit-amber)".</summary>
    public string Color { get; set; } = "var(--color-primary)";

    /// <summary>
    /// How many days per week the user aims to complete this habit (1–7).
    /// For example, 5 means "5× / week".
    /// </summary>
    public int WeeklyTargetDays { get; set; } = 7;
}
