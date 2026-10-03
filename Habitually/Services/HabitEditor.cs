using Habitually.Models;

namespace Habitually.Services;

/// <summary>A temporary form draft. Cancel leaves the real habit untouched.</summary>
public class HabitEditor
{
    public const int MaxNameLength = 30;

    public static IReadOnlyList<HabitIconOption> Icons { get; } = new HabitIconOption[]
    {
        new("🏃", "Running"), new("📚", "Reading"), new("💧", "Water"), new("🏋️", "Workout"),
        new("🌙", "Sleep"), new("✏️", "Study"), new("☕", "Coffee"), new("🔗", "Connection"),
        new("🎵", "Music"), new("🥗", "Healthy food"), new("☀️", "Outdoors"), new("🎯", "Goal"),
        new("💻", "Work"), new("🚶", "Walking"), new("🧘", "Meditation"), new("📝", "Journal")
    };

    public static IReadOnlyList<HabitColorOption> Colors { get; } = new HabitColorOption[]
    {
        new("Primary", "var(--color-primary)"), new("Red", "var(--color-danger)"),
        new("Amber", "var(--color-habit-amber)"), new("Green", "var(--color-habit-green)"),
        new("Sky", "var(--color-habit-sky)"), new("Pink", "var(--color-habit-pink)"),
        new("Purple", "var(--color-habit-purple)"), new("Teal", "var(--color-habit-teal)")
    };

    private readonly Guid _id;
    public string Name { get; set; }
    public string Emoji { get; set; }
    public string Color { get; set; }
    public int WeeklyTargetDays { get; set; }

    public bool CanSave => !string.IsNullOrWhiteSpace(Name) && Name.Trim().Length <= MaxNameLength;

    public HabitEditor(Habit? habit = null)
    {
        _id = habit?.Id ?? Guid.NewGuid();
        Name = habit?.Name ?? string.Empty;
        Emoji = habit?.Emoji ?? "🎯";
        Color = habit?.Color ?? Colors[0].Value;
        WeeklyTargetDays = habit?.WeeklyTargetDays ?? 7;
    }

    /// <summary>Build a separate model only after the form passes validation.</summary>
    public Habit CreateHabit()
    {
        if (!CanSave) throw new InvalidOperationException("A habit name must contain 1 to 30 characters.");
        if (WeeklyTargetDays is < 1 or > 7) throw new InvalidOperationException("Choose 1 to 7 days.");

        return new Habit
        {
            Id = _id,
            Name = Name.Trim(),
            Emoji = Emoji,
            Color = Color,
            WeeklyTargetDays = WeeklyTargetDays
        };
    }
}

public record HabitIconOption(string Emoji, string Label);
public record HabitColorOption(string Label, string Value);
