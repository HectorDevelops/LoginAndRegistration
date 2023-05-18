using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618 // ignores the current fields that need to be non-nulled 
#pragma warning disable CS8600 // ignores the current fields that need to be non-nulled 
#pragma warning disable CS8602 // ignores the current fields that need to be non-nulled 


namespace LoginAndRegistration.Models;
public class User
{
    [Key]
    public int UserId { get; set; }

    [Required(ErrorMessage = "is required.")]
    [MinLength(2, ErrorMessage = "must be at least 2 characters.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "is required.")]
    [MinLength(2, ErrorMessage = "must be at least 2 characters.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "is required.")]
    [EmailAddress]
    [UniqueEmail]
    public string Email { get; set; }

    [Required(ErrorMessage = "is required.")]
    [MinLength(8, ErrorMessage = "must be at least 8 characters.")]
    [DataType(DataType.Password)] // make our input the password type 
    public string Password { get; set; }

    [NotMapped]
    [DataType(DataType.Password)] // make our input the password type 
    [Compare("Password", ErrorMessage = "must match Password.")]
    public string ConfirmPassword { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

// Custom validations are needed in order to verify that the user's email are unique. 
public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("Email is required.");
        }
        // This will help us connect to our db as we ar enot in our Controller
        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));
        // check to see if there are any users that have the same email/password
        if (_context.Users.Any(user => user.Email == value.ToString()))
        {
            // if there's an error
            return new ValidationResult("Email must be unique. ");
        }
        else
        {
            return ValidationResult.Success;
        }
    }
}