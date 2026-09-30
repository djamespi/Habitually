// HabitService.cs — Central service that holds all habits and completion entries.
// All data lives in-memory for now (localStorage persistence comes later).
// A fixed Random seed ensures the same sample data every run.

using Habitually.Models;

namespace Habitually.Services;

public class HabitService
{
    // ── Private state ──────────────────────────────────────────────

    private readonly List<Habit> _habits = new();
    private readonly List<HabitEntry> _entries = new();

    /// <summary>
    /// Fires whenever habits or entries change, so UI components can re-render.
    /// Pages should subscribe in OnInitialized and unsubscribe in Dispose.
    /// </summary>
    public event Action? OnChange;

    // ── Constructor: seed sample data ──────────────────────────────

    public HabitService()
    {
        SeedHabits();
        SeedEntries();
    }

    // ── Read methods ───────────────────────────────────────────────

    /// <summary>Returns all habits (copy of the list).</summary>
    public List<Habit> GetHabits() => new(_habits);

    /// <summary>Returns all entries (copy of the list).</summary>
    public List<HabitEntry> GetEntries() => new(_entries);

    /// <summary>Checks whether a specific habit was completed on a given date.</summary>
    public bool IsCompleted(Guid habitId, DateOnly date)
    {
        return _entries.Any(e => e.HabitId == habitId && e.Date == date);
    }

    // ── Write methods ──────────────────────────────────────────────

    /// <summary>
    /// Toggles a habit's completion for the given date.
    /// If already completed, removes the entry; otherwise adds one.
    /// </summary>
    public void ToggleCompletion(Guid habitId, DateOnly date)
    {
        var existing = _entries.FirstOrDefault(
            e => e.HabitId == habitId && e.Date == date);

        if (existing is not null)
        {
            _entries.Remove(existing);   // un-check
        }
        else
        {
            _entries.Add(new HabitEntry { HabitId = habitId, Date = date }); // check
        }

        NotifyChange();
    }

    /// <summary>Adds a brand-new habit.</summary>
    public void AddHabit(Habit habit)
    {
        _habits.Add(habit);
        NotifyChange();
    }

    /// <summary>Updates an existing habit's properties (matched by Id).</summary>
    public void UpdateHabit(Habit habit)
    {
        var index = _habits.FindIndex(h => h.Id == habit.Id);
        if (index >= 0)
        {
            _habits[index] = habit;
            NotifyChange();
        }
    }

    /// <summary>Deletes a habit and all its entries.</summary>
    public void DeleteHabit(Guid id)
    {
        _habits.RemoveAll(h => h.Id == id);
        _entries.RemoveAll(e => e.HabitId == id);
        NotifyChange();
    }

    // ── Private helpers ────────────────────────────────────────────

    /// <summary>Notify subscribers that something changed.</summary>
    private void NotifyChange() => OnChange?.Invoke();

    /// <summary>
    /// Creates the 6 sample habits that match the Figma Dashboard design.
    /// Emojis and colors are taken from the Figma screenshots and DESIGN_TOKENS.txt.
    /// </summary>
    private void SeedHabits()
    {
        _habits.AddRange(new[]
        {
            new Habit
            {
                Id   = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                Name = "Morning workout", Emoji = "🏃", Color = "#db2777",  // Pink
                WeeklyTargetDays = 5
            },
            new Habit
            {
                Id   = Guid.Parse("b2222222-2222-2222-2222-222222222222"),
                Name = "Read 20 pages", Emoji = "📚", Color = "#0ea5e9",    // Sky
                WeeklyTargetDays = 7
            },
            new Habit
            {
                Id   = Guid.Parse("c3333333-3333-3333-3333-333333333333"),
                Name = "Drink 2L water", Emoji = "💧", Color = "#0ea5e9",   // Sky
                WeeklyTargetDays = 7
            },
            new Habit
            {
                Id   = Guid.Parse("d4444444-4444-4444-4444-444444444444"),
                Name = "Meditate", Emoji = "🧘", Color = "#16a34a",         // Green
                WeeklyTargetDays = 6
            },
            new Habit
            {
                Id   = Guid.Parse("e5555555-5555-5555-5555-555555555555"),
                Name = "No late-night screens", Emoji = "🌙", Color = "#7c3aed", // Purple
                WeeklyTargetDays = 5
            },
            new Habit
            {
                Id   = Guid.Parse("f6666666-6666-6666-6666-666666666666"),
                Name = "Study session", Emoji = "✏️", Color = "#f5a524",    // Amber
                WeeklyTargetDays = 5
            },
        });
    }

    /// <summary>
    /// Generates ~6 months of realistic sample entries up to today.
    /// Each habit has a different base completion rate so the heatmap
    /// and stats look varied. A fixed seed (42) keeps the data stable.
    /// Recent days form believable streaks by boosting the rate.
    /// </summary>
    private void SeedEntries()
    {
        var rng = new Random(42);   // fixed seed → same data every run
        var today = DateOnly.FromDateTime(DateTime.Now);
        var start = today.AddDays(-180);  // roughly 6 months back

        // Base completion probability for each habit (index matches _habits order).
        // These are tuned so the Dashboard screenshot looks realistic.
        double[] baseRates = { 0.50, 0.80, 0.85, 0.65, 0.75, 0.78 };

        for (int i = 0; i < _habits.Count; i++)
        {
            var habit = _habits[i];
            var baseRate = baseRates[i];

            // Walk through each day from start to today
            for (var day = start; day <= today; day = day.AddDays(1))
            {
                // Boost the rate for the most recent 30 days to create
                // believable current streaks (habits feel "active lately").
                int daysAgo = today.DayNumber - day.DayNumber;
                double rate = daysAgo < 30
                    ? Math.Min(baseRate + 0.15, 0.97)   // boosted recent window
                    : baseRate;

                // Roll the dice — if under the rate, the habit was done that day
                if (rng.NextDouble() < rate)
                {
                    _entries.Add(new HabitEntry
                    {
                        HabitId = habit.Id,
                        Date    = day
                    });
                }
            }
        }
    }
}
