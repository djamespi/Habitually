// HabitEntry.cs — Records that a habit was completed on a specific date.
// An entry only exists when the habit was done; no entry = not done.

namespace Habitually.Models;

public class HabitEntry
{
    /// <summary>Which habit this completion belongs to.</summary>
    public Guid HabitId { get; set; }

    /// <summary>The date the habit was completed (time-of-day is ignored).</summary>
    public DateOnly Date { get; set; }
}
