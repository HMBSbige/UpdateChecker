namespace UpdateChecker;

public interface IUpdateChecker
{
	string CurrentVersion { get; set; }

	string? LatestVersion { get; }

	string? LatestVersionUrl { get; }

	ValueTask<bool> CheckAsync(CancellationToken cancellationToken = default);
	ValueTask<bool> CheckAsync(HttpClient client, CancellationToken cancellationToken = default);
}
