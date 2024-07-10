using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using UpdateChecker;

namespace UnitTest;

[TestClass]
public class GitHubReleasesTest
{
	[TestMethod]
	public async Task VersionNotFoundTest()
	{
		GitHubReleasesUpdateChecker updaterChecker = new(
			@"TCPingInfoView",
			@"TCPingInfoView-Classic",
			false,
			@"1.6.0",
			tag => tag.Replace(@"v", string.Empty)
		);
		await Assert.ThrowsExceptionAsync<VersionNotFoundException>(async () => Assert.IsFalse(await updaterChecker.CheckAsync()));
	}

	[TestMethod]
	public async Task PreReleaseTestFailed()
	{
		GitHubReleasesUpdateChecker updaterChecker = new(
			@"TCPingInfoView",
			@"TCPingInfoView-Classic",
			true,
			@"1.6.0",
			tag => tag.Replace(@"v", string.Empty)
		);
		Assert.IsFalse(await updaterChecker.CheckAsync());
		Assert.IsNull(updaterChecker.LatestRelease);
	}

	[TestMethod]
	public async Task PreReleaseTestSuccess()
	{
		GitHubReleasesUpdateChecker updaterChecker = new(
			@"TCPingInfoView",
			@"TCPingInfoView-Classic",
			true,
			@"1.5.9",
			tag => tag.Replace(@"v", string.Empty)
		);
		Assert.IsTrue(await updaterChecker.CheckAsync());
		Assert.IsNotNull(updaterChecker.LatestRelease);
	}

	[TestMethod]
	public async Task ReleaseTestFailed()
	{
		GitHubReleasesUpdateChecker updaterChecker = new(
			@"TCPingInfoView",
			@"TCPingInfoView-Classic",
			false,
			@"1.6.0",
			tag => tag.Replace(@"v", string.Empty).Replace(@"-steam", string.Empty)
		);
		Assert.IsFalse(await updaterChecker.CheckAsync());
		Assert.IsNull(updaterChecker.LatestRelease);
	}

	[TestMethod]
	public async Task ReleaseTestSuccess()
	{
		GitHubReleasesUpdateChecker updaterChecker = new(
			@"TCPingInfoView",
			@"TCPingInfoView-Classic",
			false,
			@"1.5.9",
			tag => tag.Replace(@"v", string.Empty).Replace(@"-steam", string.Empty)
		);
		Assert.IsTrue(await updaterChecker.CheckAsync());
		Assert.IsNotNull(updaterChecker.LatestRelease);
	}
}
