using Domain.Entities;

namespace Application.Repositories;

public interface IMeesterRepository : IUserRepository
{
    Task<Veilingmeester?> GetMeesterByIdAsync(Guid accountId);
    Task AddAsync(Veilingmeester meester);
    void Update(Veilingmeester meester);
}