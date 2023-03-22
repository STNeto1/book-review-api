namespace book_review_api.Exceptions;

public class UnauthorizedException: Exception
{
    public UnauthorizedException() : base("Unauthorized.")
    {
    }
}