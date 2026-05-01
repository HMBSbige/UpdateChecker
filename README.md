# UpdateChecker

Channel | Status
-|-
CI | [![CI](https://github.com/HMBSbige/UpdateChecker/workflows/CI/badge.svg)](https://github.com/HMBSbige/UpdateChecker/actions)
NuGet.org | [![NuGet.org](https://img.shields.io/nuget/v/HMBSbige.UpdateChecker.svg)](https://www.nuget.org/packages/HMBSbige.UpdateChecker/)

# Usage
## GitHub Releases
```csharp
GitHubReleasesUpdateCheckerOptions options = new()
{
	Owner = @"microsoft",
	Repo = @"PowerToys",
	IsPreRelease = false,
	CurrentVersion = @"0.80.0",
	VersionScheme = new DefaultVersionScheme()
};

GitHubReleasesUpdateChecker updateChecker = new(options);
try
{
	bool result = await updateChecker.CheckAsync(default);
	// bool result = await updateChecker.CheckAsync(new HttpClient(), new CancellationToken());
	if (result)
	{
		// Update Found

		string? latestVersion = updateChecker.LatestVersion;
		string? latestVersionUrl = updateChecker.LatestVersionUrl;
	}
	else
	{
		// No newer version was found
	}
}
catch (UpdateCheckException)
{
	// Cannot find any correct tag, or current version is invalid.
}
catch (Exception)
{
	// Network, JSON, cancellation, or other unexpected exception.
}
```
