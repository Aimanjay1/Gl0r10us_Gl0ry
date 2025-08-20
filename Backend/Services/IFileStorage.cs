// Services/IFileStorage.cs  (interface you already use)
namespace BizOpsAPI.Services
{
    public interface IFileStorage
    {
        Task<string> SaveAsync(byte[] bytes, string folder, string fileName, CancellationToken ct = default);
        Task<bool>   DeleteAsync(string urlOrPath, CancellationToken ct = default);
    }
}
