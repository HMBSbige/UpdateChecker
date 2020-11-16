using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateChecker.Utils;

namespace UnitTest
{
	[TestClass]
	public class UnitTest
	{
		[TestMethod]
		public void CompareVersionTest()
		{
			Assert.IsTrue(VersionExtensions.CompareVersion(@"2.3.1.0", @"2.3.1") > 0); // wtf??? Be aware that
			Assert.IsTrue(VersionExtensions.CompareVersion(@"2.0.0.0", @"2.3.1") < 0);
			Assert.IsTrue(VersionExtensions.CompareVersion(@"1.3.1.0", @"2.3.1") < 0);
			Assert.IsTrue(VersionExtensions.CompareVersion(@"2.3.1.0", @"1.3.1") > 0);
			Assert.IsTrue(VersionExtensions.CompareVersion(@"1.2", @"1.3") < 0);
			Assert.IsTrue(VersionExtensions.CompareVersion(@"1.3", @"1.2") > 0);
			Assert.IsTrue(VersionExtensions.CompareVersion(@"1.3", @"1.3") == 0);
			Assert.IsTrue(VersionExtensions.CompareVersion(@"1.2.1", @"1.2") > 0);
			Assert.IsTrue(VersionExtensions.CompareVersion(@"2.3.1", @"2.4") < 0);
			Assert.IsTrue(VersionExtensions.CompareVersion(@"1.3.2", @"1.3.1") > 0);
		}
	}
}
