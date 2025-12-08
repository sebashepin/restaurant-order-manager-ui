using System.Text.Json;

namespace orders.Services;

public interface ILocalStorage
{
    ValueTask SetStringAsync(string key, string value);
    ValueTask<string?> GetStringAsync(string key);

    async ValueTask SetObjectAsync<T>(string key, T value, JsonSerializerOptions? options = null)
    {
        var json = JsonSerializer.Serialize(value, options ?? DefaultJsonOptions);
        await SetStringAsync(key, json);
    }

    async ValueTask<T?> GetObjectAsync<T>(string key, JsonSerializerOptions? options = null)
    {
        var json = await GetStringAsync(key);
        if (string.IsNullOrWhiteSpace(json)) return default;
        return JsonSerializer.Deserialize<T>(json!, options ?? DefaultJsonOptions);
    }

    static readonly JsonSerializerOptions DefaultJsonOptions = new(JsonSerializerDefaults.Web);
}
