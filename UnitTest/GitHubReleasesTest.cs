using System.Net;
using System.Net.Http.Headers;

namespace UnitTest;

[TestClass]
public class GitHubReleasesTest
{
	public TestContext TestContext { get; set; }

	private static HttpClient _httpClient = null!;

	[AssemblyInitialize]
	public static void AssemblyInit(TestContext context)
	{
		_httpClient = new HttpClient(new SocketsHttpHandler { AutomaticDecompression = DecompressionMethods.Brotli }, true);
		_httpClient.DefaultRequestVersion = HttpVersion.Version30;

		_httpClient.DefaultRequestHeaders.UserAgent.Clear();
		_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

		if (Environment.GetEnvironmentVariable("GITHUB_TOKEN") is { } token)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}
	}

	[TestMethod]
	public async Task VersionNotFoundTest()
	{
		GitHubReleasesUpdateChecker updaterChecker = new
		(
			@"TCPingInfoView",
			@"TCPingInfoView-Classic",
			false,
			@"1.6.0",
			tag => tag.Replace(@"v", string.Empty)
		);
		await Assert.ThrowsExactlyAsync<VersionNotFoundException>(async () => Assert.IsFalse(await updaterChecker.CheckAsync(_httpClient, TestContext.CancellationToken)));
	}

	[TestMethod]
	public async Task PreReleaseTestFailed()
	{
		GitHubReleasesUpdateChecker updaterChecker = new
		(
			@"TCPingInfoView",
			@"TCPingInfoView-Classic",
			true,
			@"1.6.0",
			tag => tag.Replace(@"v", string.Empty)
		);
		Assert.IsFalse(await updaterChecker.CheckAsync(_httpClient, TestContext.CancellationToken));
	}

	[TestMethod]
	public async Task PreReleaseTestSuccess()
	{
		GitHubReleasesUpdateChecker updaterChecker = new
		(
			@"TCPingInfoView",
			@"TCPingInfoView-Classic",
			true,
			@"1.5.9",
			tag => tag.Replace(@"v", string.Empty)
		);
		Assert.IsTrue(await updaterChecker.CheckAsync(_httpClient, TestContext.CancellationToken));
	}

	[TestMethod]
	public async Task ReleaseTestFailed()
	{
		GitHubReleasesUpdateChecker updaterChecker = new
		(
			@"TCPingInfoView",
			@"TCPingInfoView-Classic",
			false,
			@"1.6.0",
			tag => tag.Replace(@"v", string.Empty).Replace(@"-steam", string.Empty)
		);
		Assert.IsFalse(await updaterChecker.CheckAsync(_httpClient, TestContext.CancellationToken));
	}

	[TestMethod]
	public async Task ReleaseTestSuccess()
	{
		GitHubReleasesUpdateChecker updaterChecker = new
		(
			@"TCPingInfoView",
			@"TCPingInfoView-Classic",
			false,
			@"1.5.9",
			tag => tag.Replace(@"v", string.Empty).Replace(@"-steam", string.Empty)
		);
		Assert.IsTrue(await updaterChecker.CheckAsync(_httpClient, TestContext.CancellationToken));
	}
}
