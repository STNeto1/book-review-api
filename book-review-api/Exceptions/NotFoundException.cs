namespace book_review_api.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string resource) : base($"{resource} not found.")
    {
    }
}