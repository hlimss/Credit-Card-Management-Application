using System.ComponentModel.DataAnnotations;

namespace CreditCardManagement.API.DTOs;

public class UpdateUserDto
{
    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    [EmailAddress]
    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }
}
