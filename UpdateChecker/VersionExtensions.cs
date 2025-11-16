namespace UpdateChecker;

internal static class VersionExtensions
{
	public static GitHubRelease? GetLatestRelease(
		this IEnumerable<GitHubRelease> releases,
		bool isPreRelease,
		Func<string, string> tagToVersion,
		IComparer<object> versionComparer)
	{
		if (!isPreRelease)
		{
			releases = releases.Where(release => !release.IsPreRelease);
		}

		IOrderedEnumerable<GitHubRelease> ordered = releases
			.Where(release => release.TagName is not null)
			.Where(release => tagToVersion(release.TagName!).IsVersionString())
			.OrderByDescending(release => tagToVersion(release.TagName!), versionComparer);
		return ordered.FirstOrDefault();
	}

	private static bool IsVersionString(this string str)
	{
		return Version.TryParse(str, out _);
	}

	/// <summary>
	/// == 0: versions are equal
	/// > 0: version1 is greater
	/// &lt; 0: version2 is greater
	/// </summary>
	public static int CompareVersion(string? v1, string? v2)
	{
		return v1 switch
		{
			null when v2 is null => 0,
			null => -1,
			not null when v2 is null => 1,
			_ => Version.Parse(v1).CompareTo(Version.Parse(v2))
		};
	}
}
