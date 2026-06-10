using portfolio_api.Models.Dtos;

namespace portfolio_api.Services;

public interface IContactService
{
    Task SendContactAsync(ContactCreateDto dto,
        CancellationToken ct = default);
}
