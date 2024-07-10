namespace UpdateChecker.Interfaces;

public interface IUpdateChecker
{
	ValueTask<bool> CheckAsync(CancellationToken cancellationToken = default);
	ValueTask<bool> CheckAsync(HttpClient client, CancellationToken cancellationToken = default);
	string CurrentVersion { get; set; }
	string? LatestVersion { get; }
	string? LatestVersionUrl { get; }
}
