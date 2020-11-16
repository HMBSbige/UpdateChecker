using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
#if NET5_0
using System.Net.Http.Json;
#else
using System.Text.Json;
#endif

namespace UpdateChecker.Utils
{
	public static class HttpExtensions
	{
		public static async ValueTask<T?> GetJsonAsync<T>(this HttpClient client, string url, CancellationToken token)
		{
#if NET5_0
			return await client.GetFromJsonAsync<T>(url, token);
#else
			var stream = await client.GetStreamAsync(url);
			return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: token);
#endif
		}
	}
}
