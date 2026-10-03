using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Habitually.Components;

public partial class Modal : IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public bool Compact { get; set; }

    private readonly string _titleId = $"modal-title-{Guid.NewGuid():N}";
    private ElementReference _dialog;
    private IJSObjectReference? _module;
    private DotNetObjectReference<Modal>? _reference;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        _reference = DotNetObjectReference.Create(this);
        _module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/modal.js");
        await _module.InvokeVoidAsync("open", _dialog, _reference);
    }

    // All three closing actions share the parent's callback.
    [JSInvokable]
    public Task RequestClose() => OnClose.InvokeAsync();

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.InvokeVoidAsync("release", _dialog);
            await _module.DisposeAsync();
        }
        _reference?.Dispose();
    }
}
