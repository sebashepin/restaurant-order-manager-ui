using Microsoft.JSInterop;

namespace orders.Services;

public sealed class LocalStorageJs(IJSRuntime js) : ILocalStorage
{
    public async ValueTask SetStringAsync(string key, string value)
        => await js.InvokeVoidAsync("localStorage.setItem", key, value);

    public async ValueTask<string?> GetStringAsync(string key)
        => await js.InvokeAsync<string?>("localStorage.getItem", key);
}
