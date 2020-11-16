# UpdateChecker

Channel | Status
-|-
CI | [![CI](https://github.com/HMBSbige/UpdateChecker/workflows/CI/badge.svg)](https://github.com/HMBSbige/UpdateChecker/actions)
NuGet.org | [![NuGet.org](https://img.shields.io/nuget/v/HMBSbige.UpdateChecker.svg)](https://www.nuget.org/packages/HMBSbige.UpdateChecker/)

# Usage
## GitHub Releases
```csharp
var updaterChecker = new GitHubReleasesUpdateChecker(
		@"TCPingInfoView", // Owner
		@"TCPingInfoView-Classic", // Repo
		false, // Is pre-release
		@"1.6.0", // Current app version string
		tag => tag.Replace(@"v", string.Empty), // Tag to version string
		new DefaultVersionComparer() // Version comparer
);
try
{
	var res = await updaterChecker.CheckAsync(default);
	//var res = await updaterChecker.CheckAsync(new HttpClient(), new CancellationToken());
	if (res)
	{
		// Update Found

		var latestVersion = updaterChecker.LatestVersion;
		var latestVersionUrl = updaterChecker.LatestVersionUrl;
		var assetsUrl = updaterChecker.LatestRelease.assets.Select(asset => asset.browser_download_url);
	}
	else
	{
		// No newer version was found
	}
}
catch (Exception ex)
{
	// Network exception or cannot find any correct tag.
}
```
