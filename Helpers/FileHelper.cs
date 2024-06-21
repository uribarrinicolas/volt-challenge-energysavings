using System.Text.Json;

namespace EnergySavings.Helpers;

public static class FileHelper
{
    public static async Task<T> ReadJsonFileAsync<T>(string filePath)
    {
        using (var stream = File.OpenRead(filePath))
        {
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }
    }

    public static async Task WriteJsonFileAsync<T>(string filePath, T data)
    {
        var jsonString = JsonSerializer.Serialize(data);
        await File.WriteAllTextAsync(filePath, jsonString);
    }
}