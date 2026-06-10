using portfolio_api.Data;
using portfolio_api.Models;

namespace portfolio_api.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly AppDbContext _db;

    public ContactRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Contact contact, CancellationToken ct = default)
    {
        await _db.Contacts.AddAsync(contact, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
    }
}
