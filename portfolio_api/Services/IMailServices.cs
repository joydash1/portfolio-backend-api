using portfolio_api.Models.Dtos;

namespace portfolio_api.Services;

public interface IMailService
{
    Task SendMailAsync(MailDto dto, CancellationToken ct = default);
}