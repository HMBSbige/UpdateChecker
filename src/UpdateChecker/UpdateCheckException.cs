namespace UpdateChecker;

public sealed class UpdateCheckException : Exception
{
	public UpdateCheckException()
	{
	}

	public UpdateCheckException(string message) : base(message)
	{
	}

	public UpdateCheckException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
