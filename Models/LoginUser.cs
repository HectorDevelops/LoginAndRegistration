// This is used in order to use the validations per class attributes 
using System.ComponentModel.DataAnnotations;
// this is used 
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618 // ignores the current fields that need to be non-nulled 

// We do not need a separate db table for the Login portion of LogAndReg.
// Adding the Not Mapped validation to not store data
[NotMapped]
public class LoginUser 
{
    [Required(ErrorMessage = "is required.")]
    [EmailAddress]
    public string LoginEmail {get; set;}

    [Required(ErrorMessage = "is required.")]
    [MinLength(8, ErrorMessage = "must be at least 8 characters.")]
    [DataType(DataType.Password)]
    public string LoginPassword {get; set;}

}