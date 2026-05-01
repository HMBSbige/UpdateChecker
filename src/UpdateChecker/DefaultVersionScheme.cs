namespace UpdateChecker;

public sealed class DefaultVersionScheme : IVersionScheme
{
	public bool TryParse(string value, out string version)
	{
		version = value.TrimStart('v');
		return Version.TryParse(version, out _);
	}

	public int Compare(string x, string y)
	{
		return Comparer<Version>.Default.Compare(Version.Parse(x), Version.Parse(y));
	}
}
