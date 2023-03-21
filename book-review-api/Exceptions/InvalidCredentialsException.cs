namespace book_review_api.Exceptions;

public class InvalidCredentialsException: Exception
{
public InvalidCredentialsException() : base("Invalid credentials.")
    {
    }
}