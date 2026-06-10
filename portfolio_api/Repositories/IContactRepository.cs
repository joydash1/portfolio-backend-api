using portfolio_api.Models;

namespace portfolio_api.Repositories;

public interface IContactRepository
{
    Task AddAsync(Contact contact, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
