global using System.Data;
global using UpdateChecker;

[assembly: Parallelize]

namespace UnitTest;

[TestClass]
public class UnitTest
{
	[TestMethod]
	public void CompareVersionTest()
	{
		DefaultVersionComparer compare = new();
		Assert.IsGreaterThan(0, compare.Compare(@"2.3.1.0", @"2.3.1")); // wtf??? Be aware that
		Assert.IsLessThan(0, compare.Compare(@"2.0.0.0", @"2.3.1"));
		Assert.IsLessThan(0, compare.Compare(@"1.3.1.0", @"2.3.1"));
		Assert.IsGreaterThan(0, compare.Compare(@"2.3.1.0", @"1.3.1"));
		Assert.IsLessThan(0, compare.Compare(@"1.2", @"1.3"));
		Assert.IsGreaterThan(0, compare.Compare(@"1.3", @"1.2"));
		Assert.AreEqual(0, compare.Compare(@"1.3", @"1.3"));
		Assert.IsGreaterThan(0, compare.Compare(@"1.2.1", @"1.2"));
		Assert.IsLessThan(0, compare.Compare(@"2.3.1", @"2.4"));
		Assert.IsGreaterThan(0, compare.Compare(@"1.3.2", @"1.3.1"));
	}
}
