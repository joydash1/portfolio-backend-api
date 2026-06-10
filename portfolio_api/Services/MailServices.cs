using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using portfolio_api.Models.Dtos;

namespace portfolio_api.Services;

public class MailService : IMailService
{
    private readonly SmtpOptions _smtp;

    public MailService(IOptions<SmtpOptions> smtp)
    {
        _smtp = smtp.Value;
    }

    public async Task SendMailAsync(MailDto dto, CancellationToken ct = default)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_smtp.FromName, _smtp.FromAddress));
        message.To.Add(MailboxAddress.Parse(_smtp.FromAddress));
        message.Subject = "New Contact Message";

        var body = new BodyBuilder
        {
            TextBody = $@"
                        New Contact Message:

                        Name: {dto.FirstName} {dto.LastName}
                        Email: {dto.Email}

                        Message:
                        {dto.Text}"
                                };

        message.Body = body.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(
            _smtp.Host,
            _smtp.Port,
            MailKit.Security.SecureSocketOptions.StartTls,
            ct);

        if (!string.IsNullOrEmpty(_smtp.User))
        {
            await client.AuthenticateAsync(_smtp.User, _smtp.Pass, ct);
        }

        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }
}