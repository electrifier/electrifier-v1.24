using Newtonsoft.Json;
using static System.Threading.Tasks.Task;

namespace electrifier.Helpers;

public static class Json
{
    /// <summary>
    /// ToObjectAsync&lt;T&gt;
    /// </summary>
    /// <typeparam name="T">Type of <see cref="object"/>.</typeparam>
    /// <returns></returns>
    public static async Task<T?> ToObjectAsync<T>(string value) =>
        await Run(() => JsonConvert.DeserializeObject<T>(value));

    /// <summary>
    /// StringifyAsync(object value)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static async Task<string> StringifyAsync(object value)
    {
        return await Run(() => JsonConvert.SerializeObject(value));
    }
}
