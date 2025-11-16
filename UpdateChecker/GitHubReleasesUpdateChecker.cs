using System.Data;
using System.Net.Http.Json;

namespace UpdateChecker;

public class GitHubReleasesUpdateChecker : IUpdateChecker
{
	public string Owner { get; }

	public string Repo { get; }

	public bool IsPreRelease { get; set; }

	private string AllReleaseUrl => $@"https://api.github.com/repos/{Owner}/{Repo}/releases";

	public string CurrentVersion { get; set; }

	public string? LatestVersion { get; private set; }

	public string? LatestVersionUrl { get; private set; }

	private readonly Func<string, string> _tagToVersion;
	private readonly IComparer<object> _versionComparer;

	public string UserAgent { get; set; } = @"Mozilla/5.0";

	public GitHubReleasesUpdateChecker(
		string owner, string repo,
		bool isPreRelease,
		string currentVersion,
		Func<string, string>? tagToVersion = null,
		IComparer<object>? versionComparer = null)
	{
		Owner = owner;
		Repo = repo;
		IsPreRelease = isPreRelease;

		CurrentVersion = currentVersion;

		_tagToVersion = tagToVersion ?? (tag => tag);
		_versionComparer = versionComparer ?? new DefaultVersionComparer();
	}

	public async ValueTask<bool> CheckAsync(CancellationToken cancellationToken = default)
	{
		HttpClient client = new();
		client.DefaultRequestHeaders.Add(@"User-Agent", UserAgent);

		return await CheckAsync(client, cancellationToken);
	}

	public async ValueTask<bool> CheckAsync(HttpClient client, CancellationToken cancellationToken = default)
	{
		IEnumerable<GitHubRelease>? releases = await client.GetFromJsonAsync<IEnumerable<GitHubRelease>>(AllReleaseUrl, cancellationToken);
		GitHubRelease? latestRelease = releases?.GetLatestRelease(IsPreRelease, _tagToVersion, _versionComparer);

		if (latestRelease?.TagName is null)
		{
			throw new VersionNotFoundException(@"Check updates failed, maybe GitHub api is broken.");
		}

		LatestVersion = _tagToVersion(latestRelease.TagName);

		if (_versionComparer.Compare(LatestVersion, CurrentVersion) <= 0)
		{
			return false;
		}

		LatestVersionUrl = latestRelease.HtmlUrl;
		return true;
	}
}
