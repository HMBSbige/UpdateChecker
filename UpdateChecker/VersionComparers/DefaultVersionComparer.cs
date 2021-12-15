using UpdateChecker.Utils;

namespace UpdateChecker.VersionComparers;

public class DefaultVersionComparer : IComparer<object>
{
	public int Compare(object? x, object? y)
	{
		return VersionExtensions.CompareVersion(x?.ToString(), y?.ToString());
	}
}
