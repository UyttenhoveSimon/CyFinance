namespace CyFinance;

/// <summary>
/// Thrown when a Yahoo Finance request fails: the HTTP call, the response status, or reading the payload.
/// The underlying error, if any, is in <see cref="Exception.InnerException"/>.
/// </summary>
public class CyFinanceException : Exception
{
    public CyFinanceException(string message) : base(message) { }

    public CyFinanceException(string message, Exception innerException) : base(message, innerException) { }
}
