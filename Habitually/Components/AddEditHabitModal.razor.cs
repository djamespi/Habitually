using Habitually.Models;
using Habitually.Services;
using Microsoft.AspNetCore.Components;

namespace Habitually.Components;

public partial class AddEditHabitModal
{
    [Parameter] public Habit? Habit { get; set; }
    [Parameter] public EventCallback<Habit> OnSave { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    private readonly string _nameId = $"habit-name-{Guid.NewGuid():N}";
    private HabitEditor _editor = default!;

    protected override void OnInitialized()
    {
        // Copy into a draft; typing into Edit must not change the real model yet.
        _editor = new HabitEditor(Habit);
    }

    private async Task SaveAsync()
    {
        if (_editor.CanSave) await OnSave.InvokeAsync(_editor.CreateHabit());
    }
}
