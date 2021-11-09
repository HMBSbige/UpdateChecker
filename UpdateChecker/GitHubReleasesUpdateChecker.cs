using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UpdateChecker.Interfaces;
using UpdateChecker.Models.GitHub;
using UpdateChecker.Utils;
using UpdateChecker.VersionComparers;

namespace UpdateChecker
{
	public class GitHubReleasesUpdateChecker : IUpdateChecker
	{
		public string Owner { get; }
		public string Repo { get; }
		public bool IsPreRelease { get; set; }

		private string AllReleaseUrl => $@"https://api.github.com/repos/{Owner}/{Repo}/releases";

		public string CurrentVersion { get; set; }
		public string? LatestVersion { get; private set; }
		public string? LatestVersionUrl { get; private set; }
		public GitHubRelease? LatestRelease { get; private set; }

		private readonly Func<string, string> _tagToVersion;
		private readonly IComparer<object> _versionComparer;

		private const string DefaultUserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36";

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

		public async ValueTask<bool> CheckAsync(CancellationToken token)
		{
			using var client = new HttpClient();
			client.DefaultRequestHeaders.Add(@"User-Agent", DefaultUserAgent);
			return await CheckAsync(client, token);
		}

		public async ValueTask<bool> CheckAsync(HttpClient client, CancellationToken token)
		{
			var releases = await client.GetJsonAsync<IEnumerable<GitHubRelease>>(AllReleaseUrl, token);
			var latestRelease = releases?.GetLatestRelease(IsPreRelease, _tagToVersion, _versionComparer);
			if (latestRelease?.tag_name is null)
			{
				throw new VersionNotFoundException(@"Check updates failed, maybe GitHub api is broken.");
			}

			LatestVersion = _tagToVersion(latestRelease.tag_name);
			if (_versionComparer.Compare(LatestVersion, CurrentVersion) <= 0)
			{
				return false;
			}
			LatestVersionUrl = latestRelease.html_url;
			LatestRelease = latestRelease;
			return true;
		}
	}
}
