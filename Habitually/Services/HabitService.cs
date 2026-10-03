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

    /// <summary>How many habits exist.</summary>
    public int GetHabitCount() => _habits.Count;

    /// <summary>All recorded completions for one habit, including today.</summary>
    public int GetTotalCompletions(Guid habitId)
    {
        return _entries.Count(entry => entry.HabitId == habitId);
    }

    /// <summary>How many habits were completed today.</summary>
    public int GetTodayCompletedCount()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        return _habits.Count(h => IsCompleted(h.Id, today));
    }

    // ── Streak methods (delegate to StreakCalculator) ──────────────

    /// <summary>Current streak for a single habit.</summary>
    public int GetCurrentStreak(Guid habitId)
    {
        var dates = _entries.Where(e => e.HabitId == habitId).Select(e => e.Date);
        var today = DateOnly.FromDateTime(DateTime.Now);
        return StreakCalculator.GetCurrentStreak(dates, today);
    }

    /// <summary>Longest streak ever for a single habit.</summary>
    public int GetLongestStreak(Guid habitId)
    {
        var dates = _entries.Where(e => e.HabitId == habitId).Select(e => e.Date);
        return StreakCalculator.GetLongestStreak(dates);
    }

    /// <summary>Highest current streak across all habits (shown on Dashboard).</summary>
    public int GetOverallCurrentStreak()
    {
        if (_habits.Count == 0) return 0;
        return _habits.Max(h => GetCurrentStreak(h.Id));
    }

    /// <summary>Highest longest streak across all habits (shown on Dashboard).</summary>
    public int GetOverallLongestStreak()
    {
        if (_habits.Count == 0) return 0;
        return _habits.Max(h => GetLongestStreak(h.Id));
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
    /// Generates ~6 months of sample entries whose recent streaks match the
    /// Figma Dashboard screenshot exactly:
    ///
    ///   Morning workout    — streak  1, NOT done today   (yesterday only)
    ///   Read 20 pages      — streak  6, NOT done today   (days -1 to -6)
    ///   Drink 2L water     — streak 24, done today       (days  0 to -23)
    ///                        longest streak = 25          (days -50 to -26)
    ///   Meditate           — streak  1, NOT done today   (yesterday only)
    ///   No late-night…     — streak 10, done today       (days  0 to -9)
    ///   Study session      — streak  7, done today       (days  0 to -6)
    ///
    ///   Today completions: 3 of 6
    ///   Overall current streak:  24 (Drink 2L water)
    ///   Overall longest streak:  25 (Drink 2L water)
    ///
    /// Older history is filled randomly with a fixed seed (42).
    /// </summary>
    private void SeedEntries()
    {
        var rng   = new Random(42);   // fixed seed → same data every run
        var today = DateOnly.FromDateTime(DateTime.Now);
        var start = today.AddDays(-180);  // roughly 6 months back

        // ── Per-habit seeding configuration ────────────────────────
        // currentStreak   = how many recent consecutive days to force ON
        // doneToday       = whether today is included in the streak
        // longestOverride = if > 0, place an exact run of this length earlier
        // baseRate        = probability for random older days
        var configs = new[]
        {
            // Morning workout — streak 1, not done today
            new { Habit = _habits[0], CurrentStreak =  1, DoneToday = false,
                  LongestOverride = 0,  BaseRate = 0.50 },

            // Read 20 pages — streak 6, not done today
            new { Habit = _habits[1], CurrentStreak =  6, DoneToday = false,
                  LongestOverride = 0,  BaseRate = 0.80 },

            // Drink 2L water — streak 24, done today, longest 25
            new { Habit = _habits[2], CurrentStreak = 24, DoneToday = true,
                  LongestOverride = 25, BaseRate = 0.70 },

            // Meditate — streak 1, not done today
            new { Habit = _habits[3], CurrentStreak =  1, DoneToday = false,
                  LongestOverride = 0,  BaseRate = 0.60 },

            // No late-night screens — streak 10, done today
            new { Habit = _habits[4], CurrentStreak = 10, DoneToday = true,
                  LongestOverride = 0,  BaseRate = 0.70 },

            // Study session — streak 7, done today
            new { Habit = _habits[5], CurrentStreak =  7, DoneToday = true,
                  LongestOverride = 0,  BaseRate = 0.75 },
        };

        foreach (var cfg in configs)
        {
            var habitId = cfg.Habit.Id;

            // -- 1. Determine the "controlled zone" --
            // These are recent days where we force entries ON/OFF to
            // produce the exact current streak.
            // If done today, streak covers days 0..-( streak-1 ).
            // If not done today, streak covers days -1..-(streak).
            int streakEndOffset   = cfg.DoneToday ? 0 : -1;
            int streakStartOffset = streakEndOffset - cfg.CurrentStreak + 1;
            // The day just before the streak must be OFF (to "break" it)
            int breakDayOffset    = streakStartOffset - 1;

            // -- 2. Determine the "longest override zone" --
            // For Drink 2L water we place a 25-day run earlier in history.
            // Position it so it does not overlap the current streak zone.
            int overrideStart = 0, overrideEnd = 0;
            if (cfg.LongestOverride > 0)
            {
                // Place it ending 2 days before the break day
                overrideEnd   = breakDayOffset - 2;
                overrideStart = overrideEnd - cfg.LongestOverride + 1;
            }

            // -- 3. Walk every day from start to today --
            for (var day = start; day <= today; day = day.AddDays(1))
            {
                int offset = day.DayNumber - today.DayNumber; // 0 = today, -1 = yesterday…
                bool shouldAdd;

                if (offset == breakDayOffset)
                {
                    // Force OFF — this breaks the streak cleanly
                    shouldAdd = false;
                }
                else if (offset >= streakStartOffset && offset <= streakEndOffset)
                {
                    // Inside the current streak zone — force ON
                    shouldAdd = true;
                }
                else if (cfg.LongestOverride > 0 && offset >= overrideStart && offset <= overrideEnd)
                {
                    // Inside the longest-streak override zone — force ON
                    shouldAdd = true;
                }
                else if (cfg.LongestOverride > 0 && offset == overrideEnd + 1)
                {
                    // Force OFF immediately after the override to cap its length
                    shouldAdd = false;
                }
                else if (cfg.LongestOverride > 0 && offset == overrideStart - 1)
                {
                    // Force OFF just before the override zone too
                    shouldAdd = false;
                }
                else
                {
                    // Random history — use the base rate
                    shouldAdd = rng.NextDouble() < cfg.BaseRate;
                }

                if (shouldAdd)
                {
                    _entries.Add(new HabitEntry { HabitId = habitId, Date = day });
                }
            }
        }
    }
}
