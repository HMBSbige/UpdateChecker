using System.Text.Json.Serialization;

namespace UpdateChecker;

internal record GitHubRelease
{
	[JsonPropertyName("html_url")]
	public string? HtmlUrl { get; init; }

	[JsonPropertyName("tag_name")]
	public string? TagName { get; init; }

	[JsonPropertyName("prerelease")]
	public bool IsPreRelease { get; init; }
}
