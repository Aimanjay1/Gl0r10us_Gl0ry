using BizOpsAPI.Models;

namespace BizOpsAPI.Repositories
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int id);
        Task<Client> AddAsync(Client client);
        Task<Client> UpdateAsync(Client client);
        Task<bool> DeleteAsync(int id);
    }
}
