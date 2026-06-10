using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using portfolio_api.Models;
using portfolio_api.Models.Dtos;
using portfolio_api.Repositories;

namespace portfolio_api.Services;

public class ContactService : IContactService
{
    private readonly IContactRepository _repo;
    private readonly SmtpOptions _smtp;
    private readonly IWebHostEnvironment _env;

    public ContactService(IContactRepository repo, IOptions<SmtpOptions> smtp, IWebHostEnvironment env)
    {
        _repo = repo;
        _smtp = smtp.Value;
        _env = env;
    }

    public async Task SendContactAsync(ContactCreateDto dto, CancellationToken ct = default)
    {
        var contact = new Contact
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            FullName = $"{dto.FirstName} {dto.LastName}".Trim(),
            Email = dto.Email,
            Text = dto.Text
        };

        if (dto.File != null && dto.File.Length > 0)
        {
            var uploads = Path.Combine(_env.ContentRootPath, "uploads");
            Directory.CreateDirectory(uploads);
            var fileName = Path.GetRandomFileName() + Path.GetExtension(dto.File.FileName);
            var filePath = Path.Combine(uploads, fileName);
            await using var fs = new FileStream(filePath, FileMode.Create);
            await dto.File.CopyToAsync(fs, ct);
            contact.FilePath = filePath;
        }

        await _repo.AddAsync(contact, ct);
        await _repo.SaveChangesAsync(ct);

        // send mail
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtp.FromName, _smtp.FromAddress));
        message.To.Add(MailboxAddress.Parse(_smtp.FromAddress));
        message.Subject = "New contact from portfolio";

        var builder = new BodyBuilder();
        builder.TextBody = $"Name: {contact.FullName}\nEmail: {contact.Email}\n\n{contact.Text}";

        if (!string.IsNullOrEmpty(contact.FilePath) && File.Exists(contact.FilePath))
        {
            builder.Attachments.Add(contact.FilePath);
        }

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtp.Host, _smtp.Port, MailKit.Security.SecureSocketOptions.StartTls, ct);
        if (!string.IsNullOrEmpty(_smtp.User))
        {
            await client.AuthenticateAsync(_smtp.User, _smtp.Pass, ct);
        }
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }
}
