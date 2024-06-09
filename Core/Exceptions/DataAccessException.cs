namespace Core.Exceptions;

public class DataAccessException : Exception
{
    public DataAccessException(string message) : base(message)
    {
    }
}