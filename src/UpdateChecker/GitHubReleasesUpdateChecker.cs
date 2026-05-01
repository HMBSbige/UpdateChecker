using System.Net.Http.Json;

namespace UpdateChecker;

public class GitHubReleasesUpdateChecker(GitHubReleasesUpdateCheckerOptions options) : IUpdateChecker
{
	private static readonly SocketsHttpHandler SharedHandler = new();

	private readonly IVersionScheme _versionScheme = options.VersionScheme ?? new DefaultVersionScheme();

	public string Owner { get; } = options.Owner;

	public string Repo { get; } = options.Repo;

	public bool IsPreRelease { get; set; } = options.IsPreRelease;

	private string AllReleaseUrl => $@"https://api.github.com/repos/{Owner}/{Repo}/releases";

	public string CurrentVersion { get; set; } = options.CurrentVersion;

	public string? LatestVersion { get; private set; }

	public string? LatestVersionUrl { get; private set; }

	public string UserAgent { get; set; } = @"Mozilla/5.0";

	public async ValueTask<bool> CheckAsync(CancellationToken cancellationToken = default)
	{
		using HttpClient client = new(SharedHandler, disposeHandler: false);
		client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);

		return await CheckAsync(client, cancellationToken);
	}

	public async ValueTask<bool> CheckAsync(HttpClient client, CancellationToken cancellationToken = default)
	{
		LatestVersion = null;
		LatestVersionUrl = null;

		if (!_versionScheme.TryParse(CurrentVersion, out string currentVersion))
		{
			throw new UpdateCheckException(@"Current version is invalid.");
		}

		GitHubRelease[]? releases = await client.GetFromJsonAsync(AllReleaseUrl, GitHubReleaseJsonContext.Default.GitHubReleaseArray, cancellationToken);
		GitHubRelease? latestRelease = null;
		string? latestVersion = null;

		if (releases is not null)
		{
			foreach (GitHubRelease release in releases)
			{
				if (!IsPreRelease && release.IsPreRelease || release.TagName is null)
				{
					continue;
				}

				if (!_versionScheme.TryParse(release.TagName, out string version))
				{
					continue;
				}

				if (latestVersion is null || _versionScheme.Compare(version, latestVersion) > 0)
				{
					latestRelease = release;
					latestVersion = version;
				}
			}
		}

		if (latestRelease is null || latestVersion is null)
		{
			throw new UpdateCheckException(@"Check updates failed, maybe GitHub api is broken.");
		}

		LatestVersion = latestVersion;

		if (_versionScheme.Compare(LatestVersion, currentVersion) <= 0)
		{
			return false;
		}

		LatestVersionUrl = latestRelease.HtmlUrl;
		return true;
	}
}
