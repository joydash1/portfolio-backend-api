using FluentValidation;
using portfolio_api.Models.Dtos;

namespace portfolio_api.Validators;

public class ContactCreateDtoValidator : AbstractValidator<ContactCreateDto>
{
    private readonly string[] _allowedExtensions =
    [
        ".pdf",
        ".doc",
        ".docx",
        ".jpg",
        ".jpeg",
        ".png"
    ];

    public ContactCreateDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Please enter a valid email address.");

        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Message is required.")
            .MinimumLength(10)
            .MaximumLength(5000);

        RuleFor(x => x.File)
            .Must(BeValidFile)
            .When(x => x.File != null)
            .WithMessage("Invalid file type or size.");
    }

    private bool BeValidFile(IFormFile? file)
    {
        if (file == null)
            return true;

        const long maxSize = 5 * 1024 * 1024;

        if (file.Length > maxSize)
            return false;

        var extension = Path.GetExtension(file.FileName);

        return _allowedExtensions.Contains(
            extension,
            StringComparer.OrdinalIgnoreCase);
    }
}