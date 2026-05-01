namespace UpdateChecker;

public interface IVersionScheme
{
	bool TryParse(string value, out string version);

	int Compare(string x, string y);
}
