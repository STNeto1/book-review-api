using System.ComponentModel.DataAnnotations;

namespace book_review_api.Graph.Inputs;

public class LoginInput
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;
}