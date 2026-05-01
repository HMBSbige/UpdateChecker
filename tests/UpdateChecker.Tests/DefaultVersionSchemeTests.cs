namespace UpdateChecker.Tests;

public class DefaultVersionSchemeTests
{
	[Test]
	public async Task Compare_OrdersVersionStrings()
	{
		DefaultVersionScheme scheme = new();

		await Assert.That(scheme.Compare("2.3.1.0", "2.3.1")).IsGreaterThan(0);
		await Assert.That(scheme.Compare("2.0.0.0", "2.3.1")).IsLessThan(0);
		await Assert.That(scheme.Compare("1.3.1.0", "2.3.1")).IsLessThan(0);
		await Assert.That(scheme.Compare("2.3.1.0", "1.3.1")).IsGreaterThan(0);
		await Assert.That(scheme.Compare("1.2", "1.3")).IsLessThan(0);
		await Assert.That(scheme.Compare("1.3", "1.2")).IsGreaterThan(0);
		await Assert.That(scheme.Compare("1.3", "1.3")).IsEqualTo(0);
		await Assert.That(scheme.Compare("1.2.1", "1.2")).IsGreaterThan(0);
		await Assert.That(scheme.Compare("2.3.1", "2.4")).IsLessThan(0);
		await Assert.That(scheme.Compare("1.3.2", "1.3.1")).IsGreaterThan(0);
	}

	[Test]
	public async Task TryParse_TrimsLeadingVByDefault()
	{
		DefaultVersionScheme scheme = new();

		bool result = scheme.TryParse("v1.2.3", out string version);

		await Assert.That(result).IsTrue();
		await Assert.That(version).IsEqualTo("1.2.3");
	}
}
