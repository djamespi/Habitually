// HabitEntry.cs — Records that a habit was completed on a specific date.
// An entry only exists when the habit was done; no entry = not done.

namespace Habitually.Models;

/// <summary>Records that a habit was completed on a specific date.</summary>
public class HabitEntry
{
    // (HabitId, Date) must be unique when the database is added.

    /// <summary>Unique identifier for this completion record.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Which habit this completion belongs to.</summary>
    public Guid HabitId { get; set; }

    /// <summary>The date the habit was completed (time-of-day is ignored).</summary>
    public DateOnly Date { get; set; }

    /// <summary>Habit associated with this completion, when available.</summary>
    public Habit? Habit { get; set; }
}
