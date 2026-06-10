using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace portfolio_api.Models.Dtos;

public class ContactCreateDto
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;
    public IFormFile? File { get; set; }

}
