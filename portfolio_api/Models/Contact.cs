using System.ComponentModel.DataAnnotations;

namespace portfolio_api.Models;

public class Contact
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    public string Text { get; set; }

    public string? FilePath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
