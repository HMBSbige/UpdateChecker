#if NET
using System.Net.Http.Json;
#else
using System.Text.Json;
#endif

namespace UpdateChecker.Utils;

public static class HttpExtensions
{
	public static async ValueTask<T?> GetJsonAsync<T>(this HttpClient client, string url, CancellationToken token)
	{
#if NET
		return await client.GetFromJsonAsync<T>(url, token);
#else
		Stream? stream = await client.GetStreamAsync(url);
		return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: token);
#endif
	}
}
