using System.Text.Json.Serialization;

namespace UpdateChecker;

[JsonSerializable(typeof(GitHubRelease[]))]
internal sealed partial class GitHubReleaseJsonContext : JsonSerializerContext;
