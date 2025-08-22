using BizOpsAPI.DTOs;
using BizOpsAPI.Models;
using BizOpsAPI.Repositories;
using BizOpsAPI.Helpers;

namespace BizOpsAPI.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _repo;
        private readonly ICurrentUser _current;

        public ClientService(IClientRepository repo, ICurrentUser current)
        {
            _repo = repo;
            _current = current;
        }

        public async Task<IEnumerable<ClientDto>> GetAllAsync()
        {
            var userId = _current.UserId ?? throw new UnauthorizedAccessException("User not authenticated.");

            var items = await _repo.GetAllAsync();
            var mine = items.Where(c => c.UserId == userId);

            return mine.Select(c => new ClientDto
            {
                ClientId = c.ClientId,
                UserId = c.UserId,
                ClientName = c.ClientName,
                CompanyName = c.CompanyName,
                Email = c.Email
            });
        }

        public async Task<ClientDto?> GetByIdAsync(int id)
        {
            var userId = _current.UserId ?? throw new UnauthorizedAccessException("User not authenticated.");

            var c = await _repo.GetByIdAsync(id);
            if (c == null || c.UserId != userId) return null;

            return new ClientDto
            {
                ClientId = c.ClientId,
                UserId = c.UserId,
                ClientName = c.ClientName,
                CompanyName = c.CompanyName,
                Email = c.Email
            };
        }

        public async Task<ClientDto> CreateAsync(ClientCreateDto dto)
        {
            var userId = _current.UserId ?? throw new UnauthorizedAccessException("User not authenticated.");

            var entity = new Client
            {
                UserId = userId,
                ClientName = dto.ClientName,
                CompanyName = dto.CompanyName,
                CompanyAddress = dto.CompanyAddress,
                Email = dto.Email,
                ContactNumber = dto.ContactNumber,
                CreatedAt = DateTime.UtcNow,

                User = null!,                 // EF will populate
                Invoices = new List<Invoice>()// init to satisfy required nav
            };

            var created = await _repo.AddAsync(entity);

            return new ClientDto
            {
                ClientId = created.ClientId,
                UserId = created.UserId,
                ClientName = created.ClientName,
                CompanyName = created.CompanyName,
                Email = created.Email
            };
        }

        public async Task<ClientDto?> UpdateAsync(int id, ClientUpdateDto dto)
        {
            var userId = _current.UserId ?? throw new UnauthorizedAccessException("User not authenticated.");

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.UserId != userId) return null;

            entity.ClientName = dto.ClientName;
            entity.CompanyName = dto.CompanyName;
            entity.CompanyAddress = dto.CompanyAddress;
            entity.Email = dto.Email;
            entity.ContactNumber = dto.ContactNumber;

            var updated = await _repo.UpdateAsync(entity);

            return new ClientDto
            {
                ClientId = updated.ClientId,
                UserId = updated.UserId,
                ClientName = updated.ClientName,
                CompanyName = updated.CompanyName,
                Email = updated.Email
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var userId = _current.UserId ?? throw new UnauthorizedAccessException("User not authenticated.");

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.UserId != userId) return false;

            return await _repo.DeleteAsync(id);
        }
    }
}
