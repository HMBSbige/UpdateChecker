namespace UpdateChecker;

public sealed class GitHubReleasesUpdateCheckerOptions
{
	public required string Owner { get; init; }

	public required string Repo { get; init; }

	public bool IsPreRelease { get; init; }

	public required string CurrentVersion { get; init; }

	public IVersionScheme? VersionScheme { get; init; }
}
