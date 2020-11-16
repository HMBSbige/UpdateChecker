using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Threading.Tasks;
using UpdateChecker;

namespace UnitTest
{
	[TestClass]
	public class GitHubReleasesTest
	{
		[TestMethod]
		public async Task VersionNotFoundTest()
		{
			var updaterChecker = new GitHubReleasesUpdateChecker(
					@"TCPingInfoView",
					@"TCPingInfoView-Classic",
					false,
					@"1.6.0",
					tag => tag.Replace(@"v", string.Empty)
			);
			await Assert.ThrowsExceptionAsync<VersionNotFoundException>(async () =>
			{
				Assert.IsFalse(await updaterChecker.CheckAsync(default));
			});
		}

		[TestMethod]
		public async Task PreReleaseTestFailed()
		{
			var updaterChecker = new GitHubReleasesUpdateChecker(
					@"TCPingInfoView",
					@"TCPingInfoView-Classic",
					true,
					@"1.6.0",
					tag => tag.Replace(@"v", string.Empty)
			);
			Assert.IsFalse(await updaterChecker.CheckAsync(default));
			Assert.IsNull(updaterChecker.LatestRelease);
		}

		[TestMethod]
		public async Task PreReleaseTestSuccess()
		{
			var updaterChecker = new GitHubReleasesUpdateChecker(
					@"TCPingInfoView",
					@"TCPingInfoView-Classic",
					true,
					@"1.5.9",
					tag => tag.Replace(@"v", string.Empty)
			);
			Assert.IsTrue(await updaterChecker.CheckAsync(default));
			Assert.IsNotNull(updaterChecker.LatestRelease);
		}

		[TestMethod]
		public async Task ReleaseTestFailed()
		{
			var updaterChecker = new GitHubReleasesUpdateChecker(
					@"TCPingInfoView",
					@"TCPingInfoView-Classic",
					false,
					@"1.6.0",
					tag => tag.Replace(@"v", string.Empty).Replace(@"-steam", string.Empty)
			);
			Assert.IsFalse(await updaterChecker.CheckAsync(default));
			Assert.IsNull(updaterChecker.LatestRelease);
		}

		[TestMethod]
		public async Task ReleaseTestSuccess()
		{
			var updaterChecker = new GitHubReleasesUpdateChecker(
					@"TCPingInfoView",
					@"TCPingInfoView-Classic",
					false,
					@"1.5.9",
					tag => tag.Replace(@"v", string.Empty).Replace(@"-steam", string.Empty)
			);
			Assert.IsTrue(await updaterChecker.CheckAsync(default));
			Assert.IsNotNull(updaterChecker.LatestRelease);
		}
	}
}
