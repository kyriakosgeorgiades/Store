namespace Store.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string message = "Conflict occurred.") : base(message) { }
}
