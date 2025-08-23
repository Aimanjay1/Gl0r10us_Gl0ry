using BizOpsAPI.DTOs;

namespace BizOpsAPI.Services
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAllAsync();
        Task<ClientDto?> GetByIdAsync(int id);
        Task<ClientDto> CreateAsync(ClientCreateDto dto);
        Task<ClientDto?> UpdateAsync(int id, ClientUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
