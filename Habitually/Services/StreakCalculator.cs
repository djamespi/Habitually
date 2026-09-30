// StreakCalculator.cs — Pure static helper for computing streaks.
// No state, no dependencies. Easy to test and explain.

namespace Habitually.Services;

public static class StreakCalculator
{
    /// <summary>
    /// Counts consecutive completed days ending at today (or yesterday if
    /// today is not yet completed). This way a streak is never "broken"
    /// until the entire day passes without a completion.
    /// </summary>
    /// <param name="completedDates">All dates this habit was completed.</param>
    /// <param name="today">The current date.</param>
    /// <returns>Number of consecutive days in the current streak.</returns>
    public static int GetCurrentStreak(IEnumerable<DateOnly> completedDates, DateOnly today)
    {
        // Put every completed date into a fast-lookup set
        var dateSet = new HashSet<DateOnly>(completedDates);

        // Decide where to start counting:
        //   • If today is completed, start from today.
        //   • Otherwise, start from yesterday (the day isn't over yet).
        var cursor = dateSet.Contains(today) ? today : today.AddDays(-1);

        // Walk backwards counting consecutive days
        int streak = 0;
        while (dateSet.Contains(cursor))
        {
            streak++;
            cursor = cursor.AddDays(-1);
        }

        return streak;
    }

    /// <summary>
    /// Finds the longest run of consecutive completed days across all time.
    /// </summary>
    /// <param name="completedDates">All dates this habit was completed.</param>
    /// <returns>Length of the longest streak ever.</returns>
    public static int GetLongestStreak(IEnumerable<DateOnly> completedDates)
    {
        // Sort all dates so we can walk through them in order
        var sorted = completedDates.OrderBy(d => d).ToList();

        if (sorted.Count == 0)
            return 0;

        int longest = 1;   // at least one day exists
        int current = 1;   // the run we're building right now

        // Compare each date to the previous one
        for (int i = 1; i < sorted.Count; i++)
        {
            if (sorted[i].DayNumber == sorted[i - 1].DayNumber + 1)
            {
                // Consecutive — extend the current run
                current++;
            }
            else if (sorted[i].DayNumber == sorted[i - 1].DayNumber)
            {
                // Duplicate date — skip it (shouldn't happen, but be safe)
                continue;
            }
            else
            {
                // Gap — reset the current run
                current = 1;
            }

            if (current > longest)
                longest = current;
        }

        return longest;
    }
}
