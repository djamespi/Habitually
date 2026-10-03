// Native dialogs handle Escape, focus trapping, and restoring the previous focus.
const handlers = new WeakMap();

export function open(dialog, receiver) {
    const requestClose = () => receiver.invokeMethodAsync("RequestClose");
    const cancel = event => {
        event.preventDefault(); // Let Blazor remove the modal and discard its draft.
        requestClose();
    };
    const click = event => {
        if (event.target !== dialog) return;
        const bounds = dialog.getBoundingClientRect();
        // A backdrop click targets the dialog but lies outside its card.
        if (event.clientX < bounds.left || event.clientX > bounds.right ||
            event.clientY < bounds.top || event.clientY > bounds.bottom) {
            requestClose();
        }
    };
    dialog.addEventListener("cancel", cancel);
    dialog.addEventListener("click", click);
    handlers.set(dialog, { cancel, click });
    dialog.showModal();
}

export function release(dialog) {
    const registered = handlers.get(dialog);
    if (registered) {
        dialog.removeEventListener("cancel", registered.cancel);
        dialog.removeEventListener("click", registered.click);
        handlers.delete(dialog);
    }
    if (dialog.open) dialog.close();
}
