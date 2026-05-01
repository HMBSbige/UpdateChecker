using System.Net;
using System.Text;

namespace UpdateChecker.Tests;

public class GitHubReleasesUpdateCheckerTests
{
	[Test]
	public async Task CheckAsync_Throws_WhenNoVersionTag(CancellationToken cancellationToken)
	{
		GitHubReleasesUpdateChecker checker = new(new GitHubReleasesUpdateCheckerOptions
		{
			Owner = "owner",
			Repo = "repo",
			IsPreRelease = false,
			CurrentVersion = "1.0.0"
		});

		await Assert.That(CheckAsync)
			.Throws<UpdateCheckException>();

		async Task CheckAsync()
		{
			using HttpClient client = CreateClient(
				"""
				[
					{ "html_url": "https://example.invalid/releases/not-a-version", "tag_name": "not-a-version", "prerelease": false }
				]
				""");
			await checker.CheckAsync(client, cancellationToken);
		}
	}

	[Test]
	public async Task CheckAsync_IgnoresPreRelease_WhenPreReleasesDisabled(CancellationToken cancellationToken)
	{
		using HttpClient client = CreateClient(
			"""
			[
				{ "html_url": "https://example.invalid/releases/v2.0.0-preview", "tag_name": "v2.0.0", "prerelease": true },
				{ "html_url": "https://example.invalid/releases/v1.6.0", "tag_name": "v1.6.0", "prerelease": false }
			]
			""");
		GitHubReleasesUpdateChecker checker = CreateChecker("1.6.0");

		bool hasUpdate = await checker.CheckAsync(client, cancellationToken);

		await Assert.That(hasUpdate).IsFalse();
		await Assert.That(checker.LatestVersion).IsEqualTo("1.6.0");
		await Assert.That(checker.LatestVersionUrl).IsNull();
	}

	[Test]
	public async Task CheckAsync_UsesPreRelease_WhenPreReleasesEnabled(CancellationToken cancellationToken)
	{
		using HttpClient client = CreateClient(
			"""
			[
				{ "html_url": "https://example.invalid/releases/v2.0.0-preview", "tag_name": "v2.0.0", "prerelease": true },
				{ "html_url": "https://example.invalid/releases/v1.6.0", "tag_name": "v1.6.0", "prerelease": false }
			]
			""");
		GitHubReleasesUpdateChecker checker = CreateChecker("1.6.0", isPreRelease: true);

		bool hasUpdate = await checker.CheckAsync(client, cancellationToken);

		await Assert.That(hasUpdate).IsTrue();
		await Assert.That(checker.LatestVersion).IsEqualTo("2.0.0");
		await Assert.That(checker.LatestVersionUrl).IsEqualTo("https://example.invalid/releases/v2.0.0-preview");
	}

	[Test]
	public async Task CheckAsync_ReturnsTrue_ForNewStableRelease(CancellationToken cancellationToken)
	{
		using HttpClient client = CreateClient(
			"""
			[
				{ "html_url": "https://example.invalid/releases/v1.6.0", "tag_name": "v1.6.0", "prerelease": false }
			]
			""");
		GitHubReleasesUpdateChecker checker = CreateChecker("1.5.9");

		bool hasUpdate = await checker.CheckAsync(client, cancellationToken);

		await Assert.That(hasUpdate).IsTrue();
		await Assert.That(checker.LatestVersion).IsEqualTo("1.6.0");
		await Assert.That(checker.LatestVersionUrl).IsEqualTo("https://example.invalid/releases/v1.6.0");
	}

	[Test]
	public async Task CheckAsync_ClearsLatestVersionUrl_WhenLatestReleaseIsNotNewer(CancellationToken cancellationToken)
	{
		using HttpClient client = CreateClient(
			"""
			[
				{ "html_url": "https://example.invalid/releases/v1.6.0", "tag_name": "v1.6.0", "prerelease": false }
			]
			""");
		GitHubReleasesUpdateChecker checker = CreateChecker("1.5.9");

		await checker.CheckAsync(client, cancellationToken);

		checker.CurrentVersion = "1.6.0";
		bool hasUpdate = await checker.CheckAsync(client, cancellationToken);

		await Assert.That(hasUpdate).IsFalse();
		await Assert.That(checker.LatestVersion).IsEqualTo("1.6.0");
		await Assert.That(checker.LatestVersionUrl).IsNull();
	}

	[Test]
	public async Task CheckAsync_DoesNotSendRequest_WhenCurrentVersionIsInvalid(CancellationToken cancellationToken)
	{
		CountingJsonHandler handler = new(
			"""
			[
				{ "html_url": "https://example.invalid/releases/v1.6.0", "tag_name": "v1.6.0", "prerelease": false }
			]
			""");
		GitHubReleasesUpdateChecker checker = CreateChecker("not-a-version");

		await Assert.That(CheckAsync)
			.Throws<UpdateCheckException>();
		await Assert.That(handler.RequestCount).IsEqualTo(0);

		async Task CheckAsync()
		{
			using HttpClient client = CreateClient(handler);
			await checker.CheckAsync(client, cancellationToken);
		}
	}

	[Test]
	public async Task CheckAsync_UsesCustomVersionScheme(CancellationToken cancellationToken)
	{
		using HttpClient client = CreateClient(
			"""
			[
				{ "html_url": "https://example.invalid/releases/release-20260501", "tag_name": "release-20260501", "prerelease": false },
				{ "html_url": "https://example.invalid/releases/release-20260401", "tag_name": "release-20260401", "prerelease": false }
			]
			""");
		GitHubReleasesUpdateChecker checker = CreateChecker("20260415", versionScheme: new DateVersionScheme());

		bool hasUpdate = await checker.CheckAsync(client, cancellationToken);

		await Assert.That(hasUpdate).IsTrue();
		await Assert.That(checker.LatestVersion).IsEqualTo("20260501");
		await Assert.That(checker.LatestVersionUrl).IsEqualTo("https://example.invalid/releases/release-20260501");
	}

	private static HttpClient CreateClient(string json)
	{
		return CreateClient(new CountingJsonHandler(json));
	}

	private static GitHubReleasesUpdateChecker CreateChecker(
		string currentVersion,
		bool isPreRelease = false,
		IVersionScheme? versionScheme = null)
	{
		return new GitHubReleasesUpdateChecker(new GitHubReleasesUpdateCheckerOptions
		{
			Owner = "owner",
			Repo = "repo",
			IsPreRelease = isPreRelease,
			CurrentVersion = currentVersion,
			VersionScheme = versionScheme
		});
	}

	private static HttpClient CreateClient(HttpMessageHandler handler)
	{
		HttpClient client = new(handler);
		client.DefaultRequestHeaders.UserAgent.ParseAdd("UpdateChecker.Tests");
		return client;
	}

	private sealed class DateVersionScheme : IVersionScheme
	{
		public bool TryParse(string value, out string version)
		{
			const string Prefix = "release-";

			version = value.StartsWith(Prefix, StringComparison.Ordinal)
				? value.Substring(Prefix.Length)
				: value;
			return version.Length == 8 && version.All(char.IsDigit);
		}

		public int Compare(string x, string y)
		{
			return StringComparer.Ordinal.Compare(x, y);
		}
	}

	private sealed class CountingJsonHandler(string json) : HttpMessageHandler
	{
		public int RequestCount { get; private set; }

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			RequestCount++;
			HttpResponseMessage response = new(HttpStatusCode.OK)
			{
				Content = new StringContent(json, Encoding.UTF8, "application/json")
			};
			return Task.FromResult(response);
		}
	}
}
