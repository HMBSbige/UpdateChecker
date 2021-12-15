namespace UpdateChecker.Interfaces;

public interface IUpdateChecker
{
	ValueTask<bool> CheckAsync(CancellationToken token);
	ValueTask<bool> CheckAsync(HttpClient client, CancellationToken token);
	string CurrentVersion { get; set; }
	string? LatestVersion { get; }
	string? LatestVersionUrl { get; }
}
